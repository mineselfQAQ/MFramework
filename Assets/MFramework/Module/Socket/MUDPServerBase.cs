using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MFramework.Core;
using NetSocket = System.Net.Sockets.Socket;

namespace MFramework.Socket
{
    public abstract class MUDPServerBase : IDisposable
    {
        protected readonly ISocketMainThreadDispatcher Dispatcher;
        protected NetSocket Server;
        protected EndPoint EndPoint = new IPEndPoint(IPAddress.Any, 0);
        protected bool IsWaiting;

        public string IP { get; }
        public int Port { get; }
        public EndPoint ServerEndPoint { get; }
        public bool IsValid { get; private set; }

        protected MUDPServerBase(string ip, int port, ISocketMainThreadDispatcher dispatcher)
            : this(new IPEndPoint(IPAddress.Parse(ip), port), dispatcher)
        {
        }

        protected MUDPServerBase(IPEndPoint endPoint, ISocketMainThreadDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
            ServerEndPoint = endPoint;
            IP = endPoint.Address.ToString();
            Port = endPoint.Port;
        }

        public virtual void Open()
        {
            if (IsValid) return;

            Server = new NetSocket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Server.Bind(ServerEndPoint);
            IsValid = true;
            MLog.Default.D($"{nameof(MUDPServerBase)} started on {ServerEndPoint}.");
            ReceiveData();
        }

        public void Close()
        {
            if (!IsValid)
            {
                MLog.Default.W($"{nameof(MUDPServerBase)} is already closed.");
                return;
            }

            OnCloseInternal();
            ThreadPool.QueueUserWorkItem(_ => WaitForDisconnect());
        }

        public void Dispose()
        {
            Close();
        }

        protected virtual void OnCloseInternal()
        {
        }

        protected abstract void Send(UDPSendContext context, Action<EndPoint, UDPDataPack> onTrigger);
        protected abstract void ReceiveData();

        private void WaitForDisconnect()
        {
            while (IsWaiting)
            {
                Thread.Sleep(100);
            }

            try { Server?.Close(); } catch { }
            Server = null;
            IsValid = false;
            MLog.Default.D($"{nameof(MUDPServerBase)} closed.");
        }
    }
}
