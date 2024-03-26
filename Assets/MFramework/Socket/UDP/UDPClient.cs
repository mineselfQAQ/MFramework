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

        private string selfIP;
        private int selfPort;

        private Socket socket;
        private EndPoint serverEP;
        private IPEndPoint selfEP;
        private byte[] sendData;
        private byte[] receiveData;
        private string receiveStr;
        private Thread connectThread;

        private bool isConnect;

        public bool Connected { get { return isConnect; } }
        public string ReceiveStr { get { return receiveStr; } }

        internal UDPClient()
        {
            //Ipv4，使用的是数据报，也就是UDP
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            isConnect = false;
        }

        internal void Init(string serverIP, int serverPort, float interval = 5.0f, bool enableThread = true)
        {
            BindServer(serverIP, serverPort);

            //默认IP
            selfIP = MSocketUtility.GetDefaultNICIPV4Address().ToString();

            //开启线程，注意点：
            //1.服务端必须已经存在
            //2.必须向服务端先发送信息后才能连接
            CoroutineHandler.Instance.BeginCoroutineAndNotRecord(TryConnectServer(interval, enableThread));
        }

        internal void Init(IPEndPoint serverEP, float interval = 5.0f, bool enableThread = true)
        {
            BindServer(serverEP);

            //开启线程，注意点：
            //1.服务端必须已经存在
            //2.必须向服务端先发送信息后才能连接
            CoroutineHandler.Instance.BeginCoroutineAndNotRecord(TryConnectServer(interval, enableThread));
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

        private IEnumerator TryConnectServer(float interval, bool enableThread)
        {
            while (true)
            {
                if (CheckServerExists(interval))
                {
                    Send("Start");//初始检测语句

                    byte[] bytes = new byte[1024];
                    int length = socket.ReceiveFrom(bytes, ref serverEP);
                    string str = Encoding.UTF8.GetString(bytes, 0, length);

                    if (str.Contains("ConnectSucceed"))
                    {
                        string[] strs = str.Split(":");
                        string ep = strs[1];

                        string[] epStrs = ep.Split("|");
                        selfIP = epStrs[0];
                        selfPort = int.Parse(epStrs[1]);

                        selfEP = new IPEndPoint(IPAddress.Parse(selfIP), selfPort);
                    }

                    MLog.Print($"{selfEP}已成功连接");
                    if(enableThread) connectThread = InitThread(Receive);
                    yield break;
                }

                yield return new WaitForSeconds(interval);
            }
        }

        private bool CheckServerExists(float interval)
        {
            string str = UDPHandler.Instance.SendAndReceive("1", (IPEndPoint)serverEP);

            if (str != null)
            {
                isConnect = true;
                return true;
            }

            MLog.Print($"服务器连接失败，将在{interval}秒后重试", MLogType.Warning);
            return false;
        }

        public IPEndPoint GetEndPoint()
        {
            if (selfEP == null)
            {
                MLog.Print("服务器未创建，无法获取EP", MLogType.Error);
                return null;
            }

            return selfEP;
        }

        public void Send(string sendStr)
        {
            if (isConnect)
            {
                //重置数据
                sendData = new byte[1024];
                //转byte[]，因为发送使用的是字节形式
                sendData = Encoding.UTF8.GetBytes(sendStr);

                socket.SendTo(sendData, sendData.Length, SocketFlags.None, serverEP);
            }
            else
            {
                MLog.Print("未连接，无法发送消息", MLogType.Warning);
            }
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