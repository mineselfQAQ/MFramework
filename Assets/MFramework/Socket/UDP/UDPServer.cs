using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace MFramework
{
    public class UDPServer
    {
        private Socket socket;

        private EndPoint selfEP;
        private EndPoint clientEP;
        private byte[] receiveData;
        public string ReceiveStr { get { return receiveStr; } }
        private string receiveStr;

        private Thread connectThread;

        public UDPServer(string selfIP, int selfPort)
        {
            Init(selfIP, selfPort);
        }

        private void Init(string selfIP, int selfPort)
        {
            //Ipv4，使用的是数据报，也就是UDP
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //绑定自己
            //TODO:selfIP应该能自动获取
            selfEP = new IPEndPoint(IPAddress.Parse(selfIP), selfPort);
            socket.Bind(selfEP);

            //定义需要接受的EP
            clientEP = new IPEndPoint(IPAddress.Any, 0);//代表着监听所有客户端

            connectThread = InitThread(Receive);
        }
        public void Quit()
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

        protected virtual void DoAfterReceive(string receiveStr)
        {
            Debug.Log("DoNothing");
        }

        protected virtual void MainThreadDoAfterReceive(string receiveStr)
        {
            Debug.Log("DoNothing_MainThread");
        }
    }
}