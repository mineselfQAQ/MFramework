using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MFramework
{
    public abstract class MUDPServerBase
    {
        public string IP;//服务器IP
        public int Port;//服务器Port
        public EndPoint EP;//服务器EP

        protected Socket _server;
        protected EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);//任意客户端EndPoint

        protected bool isWaiting = false;

        public bool isValid { get; private set; }

        /// <summary>
        /// 服务器关闭时回调，注意：如需延迟操作，请将isWaiting设置为true，完成后置为false
        /// </summary>
        protected virtual void OnCloseInternal() { }

        protected abstract void Send(UDPSendContext context, Action<EndPoint, UDPDataPack> onTrigger);//通常版用
        protected abstract void Send(UDPSendContext context, Action<EndPoint, byte[]> onTrigger);//EZ版用
        protected abstract void ReceiveData();

        public MUDPServerBase(string ip, int port)
        {
            IP = ip;
            Port = port;
            EP = new IPEndPoint(IPAddress.Parse(ip), port);

            //InitSettings((IPEndPoint)EP);
        }
        public MUDPServerBase(IPEndPoint ep)
        {
            EP = ep;
            IP = ep.Address.ToString();
            Port = ep.Port;

            //InitSettings(ep);
        }

        private void InitSettings(IPEndPoint ep)
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _server.Bind(ep);

            isValid = true;
            MLog.Print($"{typeof(MUDPServerBase)}：服务器<{EP}>已开始监听");

            ReceiveData();
        }

        public virtual void Open()
        {
            InitSettings((IPEndPoint)EP);
        }

        public void Close()
        {
            string ep = EP.ToString();

            if (!isValid)
            {
                MLog.Print($"{typeof(MUDPServerBase)}：服务器<{ep}>已关闭，请勿重新关闭", MLogType.Warning);
                return;
            }

            OnCloseInternal();

            //Tip：只能使用线程，在OnApplicationQuit()时协程已失效
            ThreadPool.QueueUserWorkItem(_ =>
            {
                WaitForDisconnect();
            });
        }

        private void WaitForDisconnect()
        {
            //如果OnCloseInternal()设置了isWaiting=true，则会循环等待变为isWaiting=false
            while (isWaiting)
            {
                Thread.Sleep(100);//100ms检测一次
            }

            _server.Close();
            _server = null;

            isValid = false;
            MLog.Print($"{typeof(MUDPServerBase)}：服务器已关闭");
        }
    }
}
