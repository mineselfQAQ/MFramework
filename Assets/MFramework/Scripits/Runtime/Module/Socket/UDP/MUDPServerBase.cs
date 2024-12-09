using System;
using System.Net;
using System.Net.Sockets;

namespace MFramework
{
    public abstract class MUDPServerBase
    {
        public string IP;//服务器IP
        public int Port;//服务器Port
        public EndPoint EP;//服务器EP

        protected Socket _server;
        protected EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);//任意客户端EndPoint

        protected abstract void Send(SendContext context);
        protected abstract void ReceiveData();

        public MUDPServerBase(string ip, int port)
        {
            IP = ip;
            Port = port;
            EP = new IPEndPoint(IPAddress.Parse(ip), port);

            InitSettings((IPEndPoint)EP);
        }
        public MUDPServerBase(IPEndPoint ep)
        {
            EP = ep;
            IP = ep.Address.ToString();
            Port = ep.Port;

            InitSettings(ep);
        }

        public void InitSettings(IPEndPoint ep)
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _server.Bind(ep);

            MLog.Print($"{typeof(MUDPServerBase)}：服务器<{_server.LocalEndPoint}>已开始监听");

            ReceiveData();
        }

        public void Close()
        {
            if (_server != null) _server.Close();
        }
    }
}
