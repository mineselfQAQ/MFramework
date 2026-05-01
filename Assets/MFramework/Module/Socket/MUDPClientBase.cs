using System;
using System.Net;
using System.Net.Sockets;
using NetSocket = System.Net.Sockets.Socket;

namespace MFramework.Socket
{
    public abstract class MUDPClientBase : IDisposable
    {
        protected readonly ISocketMainThreadDispatcher Dispatcher;
        protected NetSocket Client;

        public EndPoint ServerEndPoint { get; protected set; }

        protected MUDPClientBase(string ip, int port, ISocketMainThreadDispatcher dispatcher)
            : this(new IPEndPoint(IPAddress.Parse(ip), port), dispatcher)
        {
        }

        protected MUDPClientBase(IPEndPoint endPoint, ISocketMainThreadDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
            ServerEndPoint = endPoint;
            Client = new NetSocket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public void Close()
        {
            if (Client == null) return;
            OnCloseInternal();
            try { Client.Close(); } catch { }
            Client = null;
        }

        public void Dispose()
        {
            Close();
        }

        protected virtual void OnCloseInternal()
        {
        }

        protected abstract void Send(UDPSendContext context, Action<UDPDataPack> onTrigger);
    }
}
