using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using MFramework.Core;
using UnityEngine;
using NetSocket = System.Net.Sockets.Socket;
using Timer = System.Timers.Timer;

namespace MFramework.Socket
{
    public sealed class MTCPServer : IDisposable
    {
        private const int HeartbeatCheckTime = 5000;

        private readonly ISocketMainThreadDispatcher _dispatcher;
        private readonly object _clientsLock = new object();

        private NetSocket _server;
        private Thread _connectThread;
        private Timer _heartbeatCheckTimer;

        public string IP { get; }
        public int Port { get; }
        public Dictionary<NetSocket, TCPClientSocketInfo> ClientInfoDic { get; private set; } = new Dictionary<NetSocket, TCPClientSocketInfo>();

        public event Action<NetSocket> OnConnect;
        public event Action<NetSocket> OnDisconnect;
        public event Action<NetSocket, TCPDataPack> OnReceive;
        public event Action<NetSocket, TCPDataPack> OnSend;

        public bool IsValid { get; private set; }
        public int BufferSize { get; private set; } = 8198;
        public int DataSize => BufferSize - TCPDataPack.HeadLength;

        public MTCPServer(string ip, int port, ISocketMainThreadDispatcher dispatcher)
        {
            IP = ip;
            Port = port;
            _dispatcher = dispatcher;

            Start();
        }

        public void SetBufferSize(int sizeKb)
        {
            int size = sizeKb * 1024 + 128;
            BufferSize = Mathf.Clamp(size, 1152, int.MaxValue);
        }

        public void SendUTF(NetSocket socket, SocketEvent type, string message, Action<NetSocket, TCPDataPack> onTrigger = null)
        {
            SendBytes(socket, type, Encoding.UTF8.GetBytes(message), onTrigger);
        }

        public void SendASCII(NetSocket socket, SocketEvent type, string message, Action<NetSocket, TCPDataPack> onTrigger = null)
        {
            SendBytes(socket, type, Encoding.ASCII.GetBytes(message), onTrigger);
        }

        public void SendBytes(NetSocket socket, SocketEvent type, byte[] buffer, Action<NetSocket, TCPDataPack> onTrigger = null)
        {
            Send(new TCPSendContext { Socket = socket, Type = (ushort)type, Buffer = buffer }, onTrigger);
        }

        public void SendEvent(NetSocket socket, SocketEvent type, Action<NetSocket, TCPDataPack> onTrigger = null)
        {
            SendBytes(socket, type, Array.Empty<byte>(), onTrigger);
        }

        public void Send(TCPSendContext context, Action<NetSocket, TCPDataPack> onTrigger = null)
        {
            if (context?.Socket == null) return;

            var dataPack = new TCPDataPack(context.Type, context.Buffer ?? Array.Empty<byte>());
            byte[] data = dataPack.Buffer;
            if (data.Length > BufferSize)
            {
                MLog.Default.W($"{nameof(MTCPServer)} payload is too large: {data.Length} bytes, max {BufferSize}.");
                return;
            }

            try
            {
                context.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, result =>
                {
                    try
                    {
                        ((NetSocket)result.AsyncState).EndSend(result);
                        _dispatcher.Post(onTrigger, context.Socket, dataPack);
                        _dispatcher.Post(OnSend, context.Socket, dataPack);
                    }
                    catch (SocketException)
                    {
                        CloseClient(context.Socket);
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                }, context.Socket);
            }
            catch (SocketException)
            {
                CloseClient(context.Socket);
            }
        }

        public void KickOut(NetSocket client)
        {
            SendEvent(client, SocketEvent.S2CKickOut, (socket, _) => CloseClient(socket));
        }

        public void KickOutAll()
        {
            foreach (NetSocket socket in SnapshotClients())
            {
                KickOut(socket);
            }
        }

        public void Close()
        {
            if (!IsValid) return;
            IsValid = false;

            StopTimer(ref _heartbeatCheckTimer);

            foreach (NetSocket socket in SnapshotClients())
            {
                CloseClient(socket);
            }

            try { _server?.Close(); } catch { }
            _server = null;

            if (_connectThread != null && _connectThread.IsAlive && Thread.CurrentThread != _connectThread)
            {
                _connectThread.Join(500);
            }

            _connectThread = null;
            OnConnect = null;
            OnDisconnect = null;
            OnReceive = null;
            OnSend = null;
        }

        public void Dispose()
        {
            Close();
        }

        private void Start()
        {
            IPAddress ipAddress = IPAddress.Parse(IP);
            _server = new NetSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _server.Bind(new IPEndPoint(ipAddress, Port));
            _server.Listen(10);
            IsValid = true;

            _connectThread = new Thread(ListenClientConnect) { IsBackground = true };
            _connectThread.Start();

            _heartbeatCheckTimer = new Timer(HeartbeatCheckTime) { AutoReset = true };
            _heartbeatCheckTimer.Elapsed += (_, _) => CheckHeadTimeOut();
            _heartbeatCheckTimer.Start();
        }

        private void ListenClientConnect()
        {
            while (IsValid)
            {
                try
                {
                    NetSocket client = _server.Accept();
                    var receiveThread = new Thread(ReceiveEvent) { IsBackground = true };
                    lock (_clientsLock)
                    {
                        ClientInfoDic.Add(client, new TCPClientSocketInfo
                        {
                            Client = client,
                            ReceiveThread = receiveThread,
                            DataBuffer = new DataBuffer(),
                            HeadTime = SocketTime.NowMilliseconds(),
                        });
                    }

                    receiveThread.Start(client);
                    _dispatcher.Post(OnConnect, client);
                }
                catch
                {
                    if (IsValid) MLog.Default.W($"{nameof(MTCPServer)} stopped accepting clients.");
                    return;
                }
            }
        }

        private void ReceiveEvent(object client)
        {
            var socket = (NetSocket)client;
            while (IsValid && TryGetInfo(socket, out TCPClientSocketInfo info))
            {
                try
                {
                    byte[] bytes = new byte[BufferSize];
                    int length = socket.Receive(bytes);
                    if (length <= 0)
                    {
                        CloseClient(socket);
                        return;
                    }

                    info.DataBuffer.AddBuffer(bytes, length);
                    TryUnpack(socket, info);
                }
                catch (SocketException)
                {
                    CloseClient(socket);
                    return;
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
            }
        }

        private void TryUnpack(NetSocket socket, TCPClientSocketInfo info)
        {
            while (info.DataBuffer.HaveBuffer)
            {
                if (!info.DataBuffer.TryUnpack(out TCPDataPack dataPack)) return;

                if (dataPack.Type == (ushort)SocketEvent.C2SHead)
                {
                    ReceiveHead(socket);
                }
                else if (dataPack.Type == (ushort)SocketEvent.C2SDisconnectRequest)
                {
                    CloseClient(socket);
                    return;
                }
                else
                {
                    _dispatcher.Post(OnReceive, socket, dataPack);
                }
            }
        }

        private void ReceiveHead(NetSocket client)
        {
            if (TryGetInfo(client, out TCPClientSocketInfo info))
            {
                info.HeadTime = SocketTime.NowMilliseconds();
            }
        }

        private void CheckHeadTimeOut()
        {
            long now = SocketTime.NowMilliseconds();
            foreach (NetSocket socket in SnapshotClients())
            {
                if (TryGetInfo(socket, out TCPClientSocketInfo info) && now - info.HeadTime > HeartbeatCheckTime)
                {
                    KickOut(socket);
                }
            }
        }

        private void CloseClient(NetSocket client)
        {
            bool removed;
            lock (_clientsLock)
            {
                removed = ClientInfoDic.Remove(client);
            }

            if (!removed) return;

            _dispatcher.Post(socket =>
            {
                OnDisconnect?.Invoke(socket);
                try { socket.Shutdown(SocketShutdown.Both); } catch { }
                try { socket.Close(); } catch { }
            }, client);
        }

        private bool TryGetInfo(NetSocket socket, out TCPClientSocketInfo info)
        {
            lock (_clientsLock)
            {
                return ClientInfoDic.TryGetValue(socket, out info);
            }
        }

        private NetSocket[] SnapshotClients()
        {
            lock (_clientsLock)
            {
                return ClientInfoDic.Keys.ToArray();
            }
        }

        private static void StopTimer(ref Timer timer)
        {
            if (timer == null) return;
            timer.Stop();
            timer.Dispose();
            timer = null;
        }
    }
}
