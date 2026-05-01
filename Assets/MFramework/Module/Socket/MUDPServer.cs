using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using MFramework.Core;
using NetSocket = System.Net.Sockets.Socket;
using Timer = System.Timers.Timer;

namespace MFramework.Socket
{
    public sealed class MUDPServer : MUDPServerBase
    {
        private const int HeartbeatCheckTime = 5000;
        private static readonly byte[] VerificationBytes = { 18, 203, 59, 38 };

        private readonly object _clientsLock = new object();
        private Timer _heartbeatCheckTimer;

        public event Action<EndPoint> OnConnect;
        public event Action<EndPoint> OnDisconnect;
        public event Action<EndPoint, UDPDataPack> OnReceive;
        public event Action<EndPoint, UDPDataPack> OnSend;

        public Dictionary<EndPoint, UDPClientSocketInfo> ClientInfoDic { get; private set; } =
            new Dictionary<EndPoint, UDPClientSocketInfo>();

        public MUDPServer(string ip, int port, ISocketMainThreadDispatcher dispatcher) : base(ip, port, dispatcher)
        {
        }

        public MUDPServer(IPEndPoint endPoint, ISocketMainThreadDispatcher dispatcher) : base(endPoint, dispatcher)
        {
        }

        public override void Open()
        {
            base.Open();
            if (_heartbeatCheckTimer == null)
            {
                StartHeartbeatCheckTimer();
            }
        }

        public void SendUTF(EndPoint endPoint, SocketEvent type, string message, Action<EndPoint, UDPDataPack> onTrigger = null)
        {
            SendBytes(endPoint, type, Encoding.UTF8.GetBytes(message), onTrigger);
        }

        public void SendASCII(EndPoint endPoint, SocketEvent type, string message, Action<EndPoint, UDPDataPack> onTrigger = null)
        {
            SendBytes(endPoint, type, Encoding.ASCII.GetBytes(message), onTrigger);
        }

        public void SendBytes(EndPoint endPoint, SocketEvent type, byte[] buffer, Action<EndPoint, UDPDataPack> onTrigger = null)
        {
            Send(new UDPSendContext { EndPoint = endPoint, Type = (ushort)type, Buffer = buffer }, onTrigger);
        }

        public void SendEvent(EndPoint endPoint, SocketEvent type, Action<EndPoint, UDPDataPack> onTrigger = null)
        {
            SendBytes(endPoint, type, Array.Empty<byte>(), onTrigger);
        }

        public void KickOutAll()
        {
            foreach (EndPoint endpoint in SnapshotClients())
            {
                KickOut(endpoint);
            }
        }

        public void KickOut(EndPoint client)
        {
            SendEvent(client, SocketEvent.S2CKickOut, (endpoint, _) => CloseClient(endpoint));
        }

        protected override void ReceiveData()
        {
            if (Server == null || !IsValid) return;

            byte[] bytes = new byte[UDPDataPack.MaxSegmentLength];
            EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            Server.BeginReceiveFrom(bytes, 0, bytes.Length, SocketFlags.None, ref remote, OnReceiveData, new ReceiveState(bytes, remote));
        }

        protected override void Send(UDPSendContext context, Action<EndPoint, UDPDataPack> onTrigger)
        {
            if (Server == null) return;

            var dataPack = new UDPDataPack(context.Type, context.Buffer ?? Array.Empty<byte>());
            foreach (byte[] packet in dataPack.Packets)
            {
                Server.BeginSendTo(packet, 0, packet.Length, SocketFlags.None, context.EndPoint, result =>
                {
                    try
                    {
                        ((NetSocket)result.AsyncState).EndSendTo(result);
                        Dispatcher.Post(onTrigger, context.EndPoint, dataPack);
                        Dispatcher.Post(OnSend, context.EndPoint, dataPack);
                    }
                    catch (SocketException ex)
                    {
                        MLog.Default.W(ex);
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                }, Server);
            }
        }

        protected override void OnCloseInternal()
        {
            foreach (EndPoint endpoint in SnapshotClients())
            {
                SendEvent(endpoint, SocketEvent.S2CDisconnectRequest);
            }

            IsWaiting = true;
            ThreadPool.QueueUserWorkItem(_ => WaitClientReplies());
        }

        private void OnReceiveData(IAsyncResult result)
        {
            try
            {
                var state = (ReceiveState)result.AsyncState;
                EndPoint remote = state.Remote;
                int length = Server.EndReceiveFrom(result, ref remote);
                if (length > 0)
                {
                    if (!TryGetInfo(remote, out UDPClientSocketInfo info))
                    {
                        HandleConnect(remote, state.Buffer, length);
                    }
                    else
                    {
                        info.DataBuffer.AddBuffer(state.Buffer, length);
                        TryUnpack(remote, info);
                    }
                }
            }
            catch (SocketException)
            {
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            finally
            {
                ReceiveData();
            }
        }

        private void HandleConnect(EndPoint endpoint, byte[] buffer, int length)
        {
            if (length < VerificationBytes.Length) return;

            byte[] received = new byte[VerificationBytes.Length];
            Array.Copy(buffer, 0, received, 0, received.Length);
            if (!received.SequenceEqual(VerificationBytes)) return;

            Server.SendTo(new byte[] { 1 }, endpoint);
            var info = new UDPClientSocketInfo
            {
                Client = endpoint,
                DataBuffer = new DataBuffer(),
                Assembler = new UDPDataPackAssembler(),
                HeadTime = SocketTime.NowMilliseconds(),
            };

            lock (_clientsLock)
            {
                ClientInfoDic[endpoint] = info;
            }

            Dispatcher.Post(OnConnect, endpoint);
        }

        private void TryUnpack(EndPoint endpoint, UDPClientSocketInfo info)
        {
            while (info.DataBuffer.HaveBuffer)
            {
                if (!info.DataBuffer.TryUnpack(info.Assembler, out UDPDataPack dataPack)) return;

                if (dataPack.Type == (ushort)SocketEvent.C2SHead)
                {
                    ReceiveHead(endpoint);
                }
                else if (dataPack.Type == (ushort)SocketEvent.C2SDisconnectRequest)
                {
                    CloseClient(endpoint);
                    SendEvent(endpoint, SocketEvent.S2CDisconnectReply);
                    return;
                }
                else if (dataPack.Type == (ushort)SocketEvent.C2SDisconnectReply)
                {
                    CloseClient(endpoint);
                    return;
                }
                else
                {
                    Dispatcher.Post(OnReceive, endpoint, dataPack);
                }
            }
        }

        private void ReceiveHead(EndPoint client)
        {
            if (TryGetInfo(client, out UDPClientSocketInfo info))
            {
                info.HeadTime = SocketTime.NowMilliseconds();
            }
        }

        private void CheckHeadTimeOut()
        {
            long now = SocketTime.NowMilliseconds();
            foreach (EndPoint endpoint in SnapshotClients())
            {
                if (TryGetInfo(endpoint, out UDPClientSocketInfo info) && now - info.HeadTime > HeartbeatCheckTime)
                {
                    KickOut(endpoint);
                }
            }
        }

        private void StartHeartbeatCheckTimer()
        {
            _heartbeatCheckTimer = new Timer(HeartbeatCheckTime) { AutoReset = true };
            _heartbeatCheckTimer.Elapsed += (_, _) => CheckHeadTimeOut();
            _heartbeatCheckTimer.Start();
        }

        private void WaitClientReplies()
        {
            int elapsed = 0;
            while (elapsed < 5000)
            {
                if (SnapshotClients().Length == 0)
                {
                    break;
                }

                Thread.Sleep(100);
                elapsed += 100;
            }

            lock (_clientsLock)
            {
                ClientInfoDic.Clear();
            }

            StopTimer(ref _heartbeatCheckTimer);
            IsWaiting = false;
        }

        private void CloseClient(EndPoint endpoint)
        {
            bool removed;
            lock (_clientsLock)
            {
                removed = ClientInfoDic.Remove(endpoint);
            }

            if (removed)
            {
                Dispatcher.Post(OnDisconnect, endpoint);
            }
        }

        private bool TryGetInfo(EndPoint endpoint, out UDPClientSocketInfo info)
        {
            lock (_clientsLock)
            {
                return ClientInfoDic.TryGetValue(endpoint, out info);
            }
        }

        private EndPoint[] SnapshotClients()
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

        private sealed class ReceiveState
        {
            public readonly byte[] Buffer;
            public readonly EndPoint Remote;

            public ReceiveState(byte[] buffer, EndPoint remote)
            {
                Buffer = buffer;
                Remote = remote;
            }
        }
    }
}
