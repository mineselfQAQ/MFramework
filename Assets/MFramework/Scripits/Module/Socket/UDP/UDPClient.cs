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
        private string serverIP;
        private int serverPort;
        private string selfIP;
        private int selfPort = -1;

        private Socket socket;
        private EndPoint serverEP;
        private IPEndPoint selfEP;
        private byte[] sendData;
        private byte[] receiveData;
        private string receiveStr;
        private Thread connectThread;

        private bool isConnect;

        public string ServerIP { get { return serverIP; } }
        public int ServerPort { get { return serverPort; } }
        public string SelfIP
        {
            get
            {
                if (selfIP == null)
                {
                    MLog.Print("未连接至服务器，无法获取本机IP", MLogType.Error);
                    return null;
                }
                return selfIP;
            }
        }
        public int SelfPort
        {
            get
            {
                if (selfPort == -1)
                {
                    MLog.Print("未连接至服务器，无法获取本机Port", MLogType.Error);
                    return -1;
                }
                return selfPort;
            }
        }

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

            //开启线程，注意点：
            //1.服务端必须已经存在
            //2.必须向服务端先发送信息后才能连接
            CoroutineManager.Instance.BeginCoroutineAndNotRecord(TryConnectServer(interval, enableThread));
        }

        internal void Init(IPEndPoint serverEP, float interval = 5.0f, bool enableThread = true)
        {
            BindServer(serverEP);

            //开启线程，注意点：
            //1.服务端必须已经存在
            //2.必须向服务端先发送信息后才能连接
            CoroutineManager.Instance.BeginCoroutineAndNotRecord(TryConnectServer(interval, enableThread));
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
                    Send("START");//初始检测语句

                    byte[] bytes = new byte[1024];
                    socket.Receive(bytes);

                    MMessage msg = MMessage.Bytes2Msg(bytes);
                    if (msg.topic == "ConnectSucceed")
                    {
                        selfIP = (string)msg.infos[0];
                        selfPort = (int)msg.infos[1];

                        selfEP = new IPEndPoint(IPAddress.Parse(selfIP), selfPort);

                        MLog.Print($"{selfEP}已成功连接");
                        if (enableThread) connectThread = InitThread(Receive);
                        yield break;
                    }
                }

                yield return new WaitForSeconds(interval);
            }
        }

        private bool CheckServerExists(float interval)
        {
            string str = UDPManager.SendAndReceive(" ", (IPEndPoint)serverEP);

            if (str == " ")
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

        public void Send(MMessage msg)
        {
            if (isConnect)
            {
                sendData = msg.Msg2Bytes();

                socket.SendTo(sendData, sendData.Length, SocketFlags.None, serverEP);
            }
            else
            {
                MLog.Print("未连接，无法发送消息", MLogType.Warning);
            }
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