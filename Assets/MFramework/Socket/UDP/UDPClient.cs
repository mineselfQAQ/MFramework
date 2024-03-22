using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System;
using System.Collections;
using UnityEngine;

namespace MFramework
{
    public class UDPClient
    {
        public string serverIP;//服务器IP
        public int serverPort;//服务器Port

        private Socket socket;
        private EndPoint selfEP;
        private EndPoint serverEP;
        private byte[] sendData;
        private byte[] receiveData;
        private string receiveStr;
        private Thread connectThread;

        public string ReceiveStr { get { return receiveStr; } }

        internal UDPClient()
        {
            //Ipv4，使用的是数据报，也就是UDP
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        internal void Init(string serverIP, int serverPort, float interval = 5.0f, bool enableThread = true)
        {
            BindServer(serverIP, serverPort);

            //开启线程，注意点：
            //1.服务端必须已经存在
            //2.必须向服务端先发送信息后才能连接
            if (enableThread)
            {
                CoroutineHandler.Instance.BeginCoroutineAndNotRecord(TryConnectServer(interval));
            }
        }

        internal void Init(IPEndPoint serverEP, float interval = 5.0f, bool enableThread = true)
        {
            BindServer(serverEP);

            //开启线程，注意点：
            //1.服务端必须已经存在
            //2.必须向服务端先发送信息后才能连接
            if (enableThread)
            {
                CoroutineHandler.Instance.BeginCoroutineAndNotRecord(TryConnectServer(interval));
            }
        }

        private IEnumerator TryConnectServer(float interval)
        {
            while (true)
            {
                if (CheckServerExists())
                {
                    MLog.Print("客户端已成功连接");
                    connectThread = InitThread(Receive);
                    yield break;
                }

                yield return new WaitForSeconds(interval);
            }
        }

        private bool CheckServerExists()
        {
            try
            {
                Send("Start");//初始检测语句
                //Debug.Log("OK");

                byte[] bytes = new byte[1024];
                int length = socket.ReceiveFrom(bytes, ref serverEP);
                string str = Encoding.UTF8.GetString(bytes, 0, length);

                if (str == "OK") return true;
                else return false;
            }
            catch
            {
                return false;
            }
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

        public void Send(string sendStr)
        {
            //重置数据
            sendData = new byte[1024];
            //转byte[]，因为发送使用的是字节形式
            sendData = Encoding.UTF8.GetBytes(sendStr);

            socket.SendTo(sendData, sendData.Length, SocketFlags.None, serverEP);
        }

        internal void Send(string sendStr, EndPoint serverEP)
        {
            //重置数据
            sendData = new byte[1024];
            //转byte[]，因为发送使用的是字节形式
            sendData = Encoding.UTF8.GetBytes(sendStr);

            socket.SendTo(sendData, sendData.Length, SocketFlags.None, serverEP);
        }

        private void Receive()
        {
            while (true)
            {
                receiveData = new byte[1024];

                int receiveLength = socket.ReceiveFrom(receiveData, ref serverEP);

                receiveStr = Encoding.UTF8.GetString(receiveData, 0, receiveLength);

                DoAfterReceive(receiveStr);

                MainThreadSynchronizationContext.Instance.Post((object state) =>
                {
                    MainThreadDoAfterReceive(receiveStr);
                });
            }
        }

        private void BindServer(string serverIP, int serverPort)
        {
            this.serverIP = serverIP;
            this.serverPort = serverPort;
            serverEP = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
        }

        private void BindServer(IPEndPoint serverEP)
        {
            this.serverIP = serverEP.Address.ToString();
            this.serverPort = serverEP.Port;
            this.serverEP = serverEP;
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