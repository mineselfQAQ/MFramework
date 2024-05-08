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
            //Ipv4，使用的是数据报，也就是UDP
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        /// <summary>
        /// 自动获取本机IP，并设置端口
        /// </summary>
        internal void Init(int selfPort)
        {
            //绑定自己
            BindSelf(MSocketUtility.GetDefaultNICIPV4Address().ToString(), selfPort);

            //定义需要接受的EP
            clientEP = new IPEndPoint(IPAddress.Any, 0);//代表着监听所有客户端

            byte[] buf = new byte[1024];
            while (socket.Available > 0)
            {
                socket.Receive(buf);
            }
            connectThread = InitThread(Receive);
        }

        /// <summary>
        /// 手动输入本机IP，并设置端口
        /// </summary>
        internal void Init(string selfIP, int selfPort)
        {
            //绑定自己
            BindSelf(selfIP, selfPort);

            //定义需要接受的EP
            clientEP = new IPEndPoint(IPAddress.Any, 0);//代表着监听所有客户端

            //byte[] buf = new byte[1024];
            //while (socket.Available > 0)
            //{
            //    socket.Receive(buf);
            //}
            connectThread = InitThread(Receive);
        }

        /// <summary>
        /// 手动输入本机EP
        /// </summary>
        internal void Init(IPEndPoint selfEP)
        {
            //绑定自己
            BindSelf(selfEP);

            //定义需要接受的EP
            clientEP = new IPEndPoint(IPAddress.Any, 0);//代表着监听所有客户端
            connectThread = InitThread(Receive);
        }

        internal void Quit()
        {
            //先关闭子线程
            if (connectThread != null)
            {
                connectThread.Abort();
            }
            //再退出Socket
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
            //重置数据
            sendData = new byte[1024];
            //转byte[]，因为发送使用的是字节形式
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

                //初始化操作
                bool flag = ResponseClientConnect(receiveStr, clientEP);
                if (flag) continue;//某客户端成功连接，此轮不应该做其它事情

                //抽象方法---受到客户端消息所需做的事
                DoAfterReceive(receiveStr);
                MainThreadSynchronizationContext.Instance.Post((object state) =>
                {
                    MainThreadDoAfterReceive(receiveStr);
                });
            }
        }

        private bool ResponseClientConnect(string receiveStr, EndPoint clientEP)
        {
            if (receiveStr == " ")//预检测
            {
                Send(" ");
                return false;
            }

            if (receiveStr == "START")//初始化
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