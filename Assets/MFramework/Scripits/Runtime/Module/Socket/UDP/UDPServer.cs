using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MFramework
{
    public class UDPServer
    {
        private string selfIP;
        private int selfPort;

        private Socket socket;
        private EndPoint selfEP;
        private EndPoint clientEP;
        private byte[] sendData;
        private byte[] receiveData;
        private string receiveStr;
        private Thread connectThread;

        public string SelfIP { get { return selfIP; } }
        public int SelfPort { get { return selfPort; } }

        public string ReceiveStr { get { return receiveStr; } }

        internal UDPServer()
        {
            //Ipv4��ʹ�õ������ݱ���Ҳ����UDP
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        /// <summary>
        /// �Զ���ȡ����IP�������ö˿�
        /// </summary>
        internal void Init(int selfPort)
        {
            //���Լ�
            BindSelf(MSocketUtility.GetDefaultNICIPV4Address().ToString(), selfPort);

            //������Ҫ���ܵ�EP
            clientEP = new IPEndPoint(IPAddress.Any, 0);//�����ż������пͻ���

            byte[] buf = new byte[1024];
            while (socket.Available > 0)
            {
                socket.Receive(buf);
            }
            connectThread = InitThread(Receive);
        }

        /// <summary>
        /// �ֶ����뱾��IP�������ö˿�
        /// </summary>
        internal void Init(string selfIP, int selfPort)
        {
            //���Լ�
            BindSelf(selfIP, selfPort);

            //������Ҫ���ܵ�EP
            clientEP = new IPEndPoint(IPAddress.Any, 0);//�����ż������пͻ���

            //byte[] buf = new byte[1024];
            //while (socket.Available > 0)
            //{
            //    socket.Receive(buf);
            //}
            connectThread = InitThread(Receive);
        }

        /// <summary>
        /// �ֶ����뱾��EP
        /// </summary>
        internal void Init(IPEndPoint selfEP)
        {
            //���Լ�
            BindSelf(selfEP);

            //������Ҫ���ܵ�EP
            clientEP = new IPEndPoint(IPAddress.Any, 0);//�����ż������пͻ���
            connectThread = InitThread(Receive);
        }

        internal void Quit()
        {
            //�ȹر����߳�
            if (connectThread != null)
            {
                connectThread.Abort();
            }
            //���˳�Socket
            if (socket != null)
            {
                socket.Close();
            }
        }

        public void Send(MMessage msg)
        {
            sendData = msg.Msg2Bytes();
            //msg = MMessage.Bytes2Msg(sendData);

            socket.SendTo(sendData, sendData.Length, SocketFlags.None, clientEP);
        }

        public void Send(string sendStr)
        {
            //��������
            sendData = new byte[1024];
            //תbyte[]����Ϊ����ʹ�õ����ֽ���ʽ
            sendData = Encoding.UTF8.GetBytes(sendStr);

            socket.SendTo(sendData, sendData.Length, SocketFlags.None, clientEP);
        }

        private void Receive()
        {
            while (true)
            {
                receiveData = new byte[1024];
                int receiveLength = socket.ReceiveFrom(receiveData, ref clientEP);

                receiveStr = Encoding.UTF8.GetString(receiveData, 0, receiveLength);

                //��ʼ������
                bool flag = ResponseClientConnect(receiveStr, clientEP);
                if (flag) continue;//ĳ�ͻ��˳ɹ����ӣ����ֲ�Ӧ������������

                //���󷽷�---�ܵ��ͻ�����Ϣ����������
                DoAfterReceive(receiveStr);
                MainThreadSynchronizationContext.Instance.Post((object state) =>
                {
                    MainThreadDoAfterReceive(receiveStr);
                });
            }
        }

        private bool ResponseClientConnect(string receiveStr, EndPoint clientEP)
        {
            if (receiveStr == " ")//Ԥ���
            {
                Send(" ");
                return false;
            }

            if (receiveStr == "START")//��ʼ��
            {
                IPEndPoint ep = (IPEndPoint)clientEP;
                string ip = ep.Address.ToString();
                int port = ep.Port;

                MMessage msg = MMessage.CreateMessage("ConnectSucceed");
                msg.AddInfo(ip, port);

                Send(msg);
                return true;
            }
            return false;
        }

        private void BindSelf(string selfIP, int selfPort)
        {
            this.selfIP = selfIP;
            this.selfPort = selfPort;
            selfEP = new IPEndPoint(IPAddress.Parse(selfIP), selfPort);
            socket.Bind(selfEP);
        }

        private void BindSelf(IPEndPoint selfEP)
        {
            this.selfIP = selfEP.Address.ToString();
            this.selfPort = selfEP.Port;
            this.selfEP = selfEP;
            socket.Bind(selfEP);
        }

        private Thread InitThread(Action action)
        {
            Thread thread = new Thread(() => action());
            thread.Start();

            return thread;
        }

        protected virtual void DoAfterReceive(string receiveStr) { }

        protected virtual void MainThreadDoAfterReceive(string receiveStr) { }
    }
}