using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using MFramework.Core;
using NetSocket = System.Net.Sockets.Socket;

namespace MFramework.Socket
{
    public sealed class MUDPClient : MUDPClientBase
    {
        private const int ConnectTimeout = 3000;
        private const int HeartbeatInterval = 2000;
        private static readonly byte[] VerificationBytes = { 18, 203, 59, 38 };

        private readonly DataBuffer _dataBuffer = new DataBuffer();
        private readonly UDPDataPackAssembler _assembler = new UDPDataPackAssembler();

        private Timer _connectTimeoutTimer;
        private Timer _heartbeatTimer;

        public event Action OnConnectSuccess;
        public event Action OnConnectError;
        public event Action OnDisconnect;
        public event Action<UDPDataPack> OnReceive;
        public event Action<UDPDataPack> OnSend;
        public event Action<Exception> OnError;

        public bool IsConnect { get; private set; }
        public bool IsConnecting { get; private set; }

        public MUDPClient(string ip, int port, ISocketMainThreadDispatcher dispatcher) : base(ip, port, dispatcher)
        {
        }

        public MUDPClient(IPEndPoint endPoint, ISocketMainThreadDispatcher dispatcher) : base(endPoint, dispatcher)
        {
        }

        public void Connect(Action onSuccess = null, Action onError = null)
        {
            if (IsConnect || IsConnecting) return;
            IsConnecting = true;
            bool completed = false;

            void Complete(bool success)
            {
                if (completed) return;
                completed = true;
                IsConnecting = false;
                StopTimer(ref _connectTimeoutTimer);
                if (success)
                {
                    IsConnect = true;
                    StartHeartbeat();
                    ReceiveData();
                    Dispatcher.Post(onSuccess);
                    Dispatcher.Post(OnConnectSuccess);
                }
                else
                {
                    Dispatcher.Post(onError);
                    Dispatcher.Post(OnConnectError);
                }
            }

            _connectTimeoutTimer = new Timer(ConnectTimeout) { AutoReset = false };
            _connectTimeoutTimer.Elapsed += (_, _) => Complete(false);
            _connectTimeoutTimer.Start();

            Client.BeginSendTo(VerificationBytes, 0, VerificationBytes.Length, SocketFlags.None, ServerEndPoint, result =>
            {
                try
                {
                    Client.EndSendTo(result);
                    Client.ReceiveTimeout = ConnectTimeout;
                    byte[] response = new byte[1];
                    EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                    int length = Client.ReceiveFrom(response, ref remote);
                    Client.ReceiveTimeout = 0;
                    Complete(length == 1 && response[0] == 1);
                }
                catch (Exception ex)
                {
                    MLog.Default.W(ex);
                    Complete(false);
                }
            }, null);
        }

        public void SendUTF(SocketEvent type, string message, Action<UDPDataPack> onTrigger = null)
        {
            SendBytes(type, Encoding.UTF8.GetBytes(message), onTrigger);
        }

        public void SendASCII(SocketEvent type, string message, Action<UDPDataPack> onTrigger = null)
        {
            SendBytes(type, Encoding.ASCII.GetBytes(message), onTrigger);
        }

        public void SendBytes(SocketEvent type, byte[] buffer, Action<UDPDataPack> onTrigger = null)
        {
            Send(new UDPSendContext { EndPoint = ServerEndPoint, Type = (ushort)type, Buffer = buffer }, onTrigger);
        }

        public void SendEvent(SocketEvent type, Action<UDPDataPack> onTrigger = null)
        {
            SendBytes(type, Array.Empty<byte>(), onTrigger);
        }

        public void Disconnect()
        {
            SendEvent(SocketEvent.C2SDisconnectRequest);
            OnDisconnectInternal();
        }

        protected override void Send(UDPSendContext context, Action<UDPDataPack> onTrigger)
        {
            if (Client == null) return;

            var dataPack = new UDPDataPack(context.Type, context.Buffer ?? Array.Empty<byte>());
            foreach (byte[] packet in dataPack.Packets)
            {
                Client.BeginSendTo(packet, 0, packet.Length, SocketFlags.None, context.EndPoint, result =>
                {
                    try
                    {
                        ((NetSocket)result.AsyncState).EndSendTo(result);
                        Dispatcher.Post(onTrigger, dataPack);
                        Dispatcher.Post(OnSend, dataPack);
                    }
                    catch (Exception ex)
                    {
                        OnErrorInternal(ex);
                    }
                }, Client);
            }
        }

        protected override void OnCloseInternal()
        {
            OnDisconnectInternal();
        }

        private void ReceiveData()
        {
            if (Client == null || !IsConnect) return;

            byte[] bytes = new byte[UDPDataPack.MaxSegmentLength];
            EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            Client.BeginReceiveFrom(bytes, 0, bytes.Length, SocketFlags.None, ref remote, OnReceiveData, new ReceiveState(bytes, remote));
        }

        private void OnReceiveData(IAsyncResult result)
        {
            try
            {
                var state = (ReceiveState)result.AsyncState;
                EndPoint remote = state.Remote;
                int length = Client.EndReceiveFrom(result, ref remote);
                if (length > 0)
                {
                    _dataBuffer.AddBuffer(state.Buffer, length);
                    TryUnpack();
                }
            }
            catch (Exception ex)
            {
                OnErrorInternal(ex);
            }
            finally
            {
                ReceiveData();
            }
        }

        private void TryUnpack()
        {
            while (_dataBuffer.HaveBuffer)
            {
                if (!_dataBuffer.TryUnpack(_assembler, out UDPDataPack dataPack)) return;

                if (dataPack.Type == (ushort)SocketEvent.S2CKickOut ||
                    dataPack.Type == (ushort)SocketEvent.S2CDisconnectRequest)
                {
                    OnDisconnectInternal();
                    return;
                }

                Dispatcher.Post(OnReceive, dataPack);
            }
        }

        private void StartHeartbeat()
        {
            _heartbeatTimer = new Timer(HeartbeatInterval) { AutoReset = true };
            _heartbeatTimer.Elapsed += (_, _) => SendEvent(SocketEvent.C2SHead);
            _heartbeatTimer.Start();
        }

        private void OnErrorInternal(Exception ex)
        {
            Dispatcher.Post(OnError, ex);
        }

        private void OnDisconnectInternal()
        {
            if (!IsConnect && Client == null) return;

            IsConnect = false;
            StopTimer(ref _heartbeatTimer);
            StopTimer(ref _connectTimeoutTimer);
            Dispatcher.Post(OnDisconnect);
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
