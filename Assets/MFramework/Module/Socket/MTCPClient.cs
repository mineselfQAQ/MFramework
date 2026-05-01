using System;
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
    public sealed class MTCPClient : IDisposable
    {
        private const int ConnectTimeout = 3000;
        private const int SendTimeout = 3000;
        private const int ReceiveTimeout = 3000;
        private const int HeartbeatInterval = 2000;
        private const int ReconnectMaxCount = 3;

        private readonly ISocketMainThreadDispatcher _dispatcher;
        private readonly DataBuffer _dataBuffer = new DataBuffer();

        private NetSocket _client;
        private Thread _receiveThread;
        private Timer _connectTimeoutTimer;
        private Timer _heartbeatTimer;

        public string IP { get; }
        public int Port { get; }

        public event Action OnConnectSuccess;
        public event Action OnConnectError;
        public event Action<int> OnReConnectSuccess;
        public event Action<int> OnReConnectError;
        public event Action<int> OnReconnecting;
        public event Action OnDisconnect;
        public event Action<TCPDataPack> OnReceive;
        public event Action<TCPDataPack> OnSend;
        public event Action<SocketException> OnError;

        public bool IsConnect { get; private set; }
        public bool IsConnecting { get; private set; }
        public bool IsReconnecting { get; private set; }
        public int BufferSize { get; private set; } = 8198;
        public int DataSize => BufferSize - TCPDataPack.HeadLength;

        public MTCPClient(string ip, int port, ISocketMainThreadDispatcher dispatcher)
        {
            IP = ip;
            Port = port;
            _dispatcher = dispatcher;
        }

        public void SetBufferSize(int sizeKb)
        {
            int size = sizeKb * 1024 + TCPDataPack.HeadLength;
            BufferSize = Mathf.Clamp(size, 1030, int.MaxValue);
        }

        public void Connect(Action onSuccess = null, Action onError = null)
        {
            if (IsConnect)
            {
                MLog.Default.W($"{nameof(MTCPClient)} is already connected.");
                return;
            }

            if (IsConnecting) return;
            IsConnecting = true;

            void Complete(bool success)
            {
                IsConnecting = false;
                StopTimer(ref _connectTimeoutTimer);

                if (success)
                {
                    _dispatcher.Post(onSuccess);
                    _dispatcher.Post(OnConnectSuccess);
                }
                else
                {
                    _dispatcher.Post(onError);
                    _dispatcher.Post(OnConnectError);
                }
            }

            _connectTimeoutTimer = new Timer(ConnectTimeout) { AutoReset = false };
            _connectTimeoutTimer.Elapsed += (_, _) => Complete(false);
            _connectTimeoutTimer.Start();

            try
            {
                _client = new NetSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, SendTimeout);
                _client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, ReceiveTimeout);
                var endpoint = new IPEndPoint(IPAddress.Parse(IP), Port);

                _client.BeginConnect(endpoint, result =>
                {
                    try
                    {
                        var socket = (NetSocket)result.AsyncState;
                        socket.EndConnect(result);
                        IsConnect = true;
                        StartHeartbeat();
                        StartReceiveThread();
                        Complete(true);
                    }
                    catch (SocketException)
                    {
                        Complete(false);
                    }
                    catch (ObjectDisposedException)
                    {
                        Complete(false);
                    }
                }, _client);
            }
            catch (SocketException)
            {
                Complete(false);
            }
        }

        public void ReConnect(int count = ReconnectMaxCount, int index = 0)
        {
            IsReconnecting = true;
            count--;
            index++;

            if (count < 0)
            {
                IsReconnecting = false;
                OnDisconnectInternal();
                return;
            }

            _dispatcher.Post(OnReconnecting, index);
            Connect(
                () =>
                {
                    IsReconnecting = false;
                    _dispatcher.Post(OnReConnectSuccess, index);
                },
                () =>
                {
                    _dispatcher.Post(OnReConnectError, index);
                    ReConnect(count, index);
                });
        }

        public void SendUTF(SocketEvent type, string message, Action<TCPDataPack> onTrigger = null)
        {
            SendBytes(type, Encoding.UTF8.GetBytes(message), onTrigger);
        }

        public void SendASCII(SocketEvent type, string message, Action<TCPDataPack> onTrigger = null)
        {
            SendBytes(type, Encoding.ASCII.GetBytes(message), onTrigger);
        }

        public void SendBytes(SocketEvent type, byte[] buffer, Action<TCPDataPack> onTrigger = null)
        {
            Send(new TCPSendContext { Socket = _client, Type = (ushort)type, Buffer = buffer }, onTrigger);
        }

        public void SendEvent(SocketEvent type, Action<TCPDataPack> onTrigger = null)
        {
            SendBytes(type, Array.Empty<byte>(), onTrigger);
        }

        public void Send(TCPSendContext context, Action<TCPDataPack> onTrigger = null)
        {
            if (context?.Socket == null || !IsConnect) return;

            var dataPack = new TCPDataPack(context.Type, context.Buffer ?? Array.Empty<byte>());
            byte[] data = dataPack.Buffer;
            if (data.Length > BufferSize)
            {
                MLog.Default.W($"{nameof(MTCPClient)} payload is too large: {data.Length} bytes, max {BufferSize}.");
                return;
            }

            try
            {
                context.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, result =>
                {
                    try
                    {
                        ((NetSocket)result.AsyncState).EndSend(result);
                        _dispatcher.Post(onTrigger, dataPack);
                        _dispatcher.Post(OnSend, dataPack);
                    }
                    catch (SocketException ex)
                    {
                        OnErrorInternal(ex);
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                }, context.Socket);
            }
            catch (SocketException ex)
            {
                OnErrorInternal(ex);
            }
        }

        public void Disconnect()
        {
            SendEvent(SocketEvent.C2SDisconnectRequest);
            OnDisconnectInternal();
        }

        public void Close()
        {
            if (!IsConnect && _client == null) return;
            IsConnect = false;

            StopTimer(ref _heartbeatTimer);
            StopTimer(ref _connectTimeoutTimer);

            try { _client?.Shutdown(SocketShutdown.Both); } catch { }
            try { _client?.Close(); } catch { }
            _client = null;

            if (_receiveThread != null && _receiveThread.IsAlive && Thread.CurrentThread != _receiveThread)
            {
                _receiveThread.Join(500);
            }

            _receiveThread = null;
        }

        public void Dispose()
        {
            Close();
        }

        private void StartHeartbeat()
        {
            _heartbeatTimer = new Timer(HeartbeatInterval) { AutoReset = true };
            _heartbeatTimer.Elapsed += (_, _) => SendEvent(SocketEvent.C2SHead);
            _heartbeatTimer.Start();
        }

        private void StartReceiveThread()
        {
            _receiveThread = new Thread(ReceiveEvent) { IsBackground = true };
            _receiveThread.Start();
        }

        private void ReceiveEvent()
        {
            while (IsConnect)
            {
                try
                {
                    byte[] bytes = new byte[BufferSize];
                    int length = _client.Receive(bytes);
                    if (length <= 0)
                    {
                        OnDisconnectInternal();
                        return;
                    }

                    _dataBuffer.AddBuffer(bytes, length);
                    TryUnpack();
                }
                catch (SocketException ex)
                {
                    OnErrorInternal(ex);
                    return;
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
            }
        }

        private void TryUnpack()
        {
            while (_dataBuffer.HaveBuffer)
            {
                if (!_dataBuffer.TryUnpack(out TCPDataPack dataPack)) return;

                if (dataPack.Type == (ushort)SocketEvent.S2CKickOut)
                {
                    OnDisconnectInternal();
                    return;
                }

                _dispatcher.Post(OnReceive, dataPack);
            }
        }

        private void OnErrorInternal(SocketException ex)
        {
            Close();
            _dispatcher.Post(OnError, ex);

            if (!IsReconnecting)
            {
                ReConnect();
            }
        }

        private void OnDisconnectInternal()
        {
            Close();
            _dispatcher.Post(OnDisconnect);
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
