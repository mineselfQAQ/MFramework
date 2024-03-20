using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MFramework
{
    public class UDPServer
    {
        public string selfIP;
        public int selfPort;

        private Socket socket;
        private EndPoint selfEP;
        private EndPoint clientEP;
        private byte[] sendData;
        private byte[] receiveData;
        private string receiveStr;
        private Thread connectThread;

        public string ReceiveStr { get { return receiveStr; } }

        internal UDPServer()
        {
            //Ipv4，使用的是数据报，也就是UDP
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        internal void Init(string selfIP, int selfPort)
        {
            //绑定自己
            //TODO:selfIP应该能自动获取
            this.selfIP = selfIP;
            this.selfPort = selfPort;
            selfEP = new IPEndPoint(IPAddress.Parse(selfIP), selfPort);
            socket.Bind(selfEP);

            //定义需要接受的EP
            clientEP = new IPEndPoint(IPAddress.Any, 0);//代表着监听所有客户端

            connectThread = InitThread(Receive);
        }

        internal void Quit()
        {
            //先关闭子线程
            if (connectThread != null)
            {
                connectThread.Interrupt();
            }
            //再退出Socket
            if (socket != null)
            {
                socket.Close();
            }
        }

        public void Send(string sendStr)
        {
            //重置数据
            sendData = new byte[1024];
            //转byte[]，因为发送使用的是字节形式
            sendData = Encoding.UTF8.GetBytes(sendStr);

            try
            {
                socket.SendTo(sendData, sendData.Length, SocketFlags.None, clientEP);
            }
            catch
            {
                throw new Exception("未找到clientEP");
            }
        }

        private void Receive()
        {
            while (true)
            {
                receiveData = new byte[1024];

                int receiveLength = socket.ReceiveFrom(receiveData, ref clientEP);

                receiveStr = Encoding.UTF8.GetString(receiveData, 0, receiveLength);

                DoAfterReceive(receiveStr);
                MainThreadSynchronizationContext.Instance.Post((object state) =>
                {
                    MainThreadDoAfterReceive(receiveStr);
                });
            }
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