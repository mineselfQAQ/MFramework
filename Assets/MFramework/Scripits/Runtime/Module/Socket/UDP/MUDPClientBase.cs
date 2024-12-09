using System.Net.Sockets;
using System.Net;
using System;

namespace MFramework
{
    public abstract class MUDPClientBase
    {
        protected Socket _client;
        protected EndPoint _serverEP;//服务器地址

        public MUDPClientBase(string ip, int port)
        {
            //服务器参数设置
            var ep = new IPEndPoint(IPAddress.Parse(ip), port);

            InitSettings(ep);
        }
        public MUDPClientBase(IPEndPoint ep)
        {
            InitSettings(ep);
        }

        public void InitSettings(IPEndPoint ep)
        {
            _serverEP = ep;
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        protected abstract void Send(SendContext context, Action<SocketDataPack> onTrigger);
    }
}
