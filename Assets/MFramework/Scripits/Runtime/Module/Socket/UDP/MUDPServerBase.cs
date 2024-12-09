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

        public bool isValid { get; private set; }

        /// <summary>
        /// 服务器关闭时回调
        /// </summary>
        protected virtual void OnCloseInternal() { }

        protected abstract void Send(UDPSendContext context, Action<EndPoint, SocketDataPack> onTrigger);//通常版用
        protected abstract void Send(UDPSendContext context, Action<EndPoint, byte[]> onTrigger);//EZ版用
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

        private void InitSettings(IPEndPoint ep)
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _server.Bind(ep);

            isValid = true;
            MLog.Print($"{typeof(MUDPServerBase)}：服务器<{EP}>已开始监听");

            ReceiveData();
        }

        public void Close()
        {
            string ep = EP.ToString();

            if (!isValid)
            {
                MLog.Print($"{typeof(MUDPServerBase)}：服务器<{ep}>已关闭，请勿重新关闭", MLogType.Warning);
                return;
            }
            isValid = false;

            OnCloseInternal();

            EP = null;
            endPoint = null;

            _server.Close();
            _server = null;
            MLog.Print($"{typeof(MUDPServerBase)}：服务器<{ep}>已关闭");
        }
    }
}
