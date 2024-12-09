using System;
using System.Net;
using System.Net.Sockets;

namespace MFramework
{
    public abstract class MUDPServerBase
    {
        public string IP;//������IP
        public int Port;//������Port
        public EndPoint EP;//������EP

        protected Socket _server;
        protected EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);//����ͻ���EndPoint

        public bool isValid { get; private set; }

        /// <summary>
        /// �������ر�ʱ�ص�
        /// </summary>
        protected virtual void OnCloseInternal() { }

        protected abstract void Send(UDPSendContext context, Action<EndPoint, SocketDataPack> onTrigger);//ͨ������
        protected abstract void Send(UDPSendContext context, Action<EndPoint, byte[]> onTrigger);//EZ����
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
            MLog.Print($"{typeof(MUDPServerBase)}��������<{EP}>�ѿ�ʼ����");

            ReceiveData();
        }

        public void Close()
        {
            string ep = EP.ToString();

            if (!isValid)
            {
                MLog.Print($"{typeof(MUDPServerBase)}��������<{ep}>�ѹرգ��������¹ر�", MLogType.Warning);
                return;
            }
            isValid = false;

            OnCloseInternal();

            EP = null;
            endPoint = null;

            _server.Close();
            _server = null;
            MLog.Print($"{typeof(MUDPServerBase)}��������<{ep}>�ѹر�");
        }
    }
}
