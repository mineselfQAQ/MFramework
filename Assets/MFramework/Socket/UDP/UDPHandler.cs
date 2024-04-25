using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MFramework
{
    public class UDPHandler : MonoSingleton<UDPHandler>
    {
        private UDPServer server;
        private List<UDPClient> clients;

        #region StaticMethod
        /// <summary>
        /// 向服务器仅发送一条消息
        /// </summary>
        public static void Send(string sendStr, IPEndPoint serverEP)
        {
            using (UdpClient udpClient = new UdpClient())
            {
                byte[] sendData = Encoding.UTF8.GetBytes(sendStr);
                udpClient.Send(sendData, sendData.Length, serverEP);
            }
        }

        /// <summary>
        /// 向服务器发送并接受消息
        /// </summary>
        public static string SendAndReceive(string sendStr, IPEndPoint serverEP)
        {
            using (UdpClient udpClient = new UdpClient())
            {
                try
                {
                    //接受是时会等待1秒，否则超时(Tip:如果通信正常，耗时忽略不计，1秒还没收到必然存在问题)
                    udpClient.Client.ReceiveTimeout = 1000;

                    byte[] sendData = Encoding.UTF8.GetBytes(sendStr);
                    udpClient.Send(sendData, sendData.Length, serverEP);

                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receiveData = udpClient.Receive(ref remoteEP);
                    string receivedMessage = Encoding.UTF8.GetString(receiveData);

                    return receivedMessage;
                }
                catch (SocketException ex)
                {
                    MLog.Print(ex.Message, MLogType.Error);
                    return null;
                }
            }
        }
        #endregion

        private UDPHandler() 
        {
            clients = new List<UDPClient>();
        }

        private void Update()
        {
            MainThreadSynchronizationContext.Instance.Update();
        }

        public void OnApplicationQuit()
        {
            if (server != null) server.Quit();

            foreach (var client in clients)
            {
                client.Quit();
            }
        }

        public bool CheckConnection(string sendStr, IPEndPoint serverEP)
        {
            using (UdpClient udpClient = new UdpClient())
            {
                try
                {
                    byte[] sendData = Encoding.UTF8.GetBytes(sendStr);
                    udpClient.Send(sendData, sendData.Length, serverEP);

                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receiveData = udpClient.Receive(ref remoteEP);

                    return true;
                }
                catch (SocketException)
                {
                    return false;
                }
            }
        }

        public T CreateServer<T>(int selfPort) where T : UDPServer, new()
        {
            if (server != null)
            {
                MLog.Print("服务端只能有唯一一个，不能多次创建", MLogType.Warning);
                return null;
            }
            server = new T();
            server.Init(selfPort);

            return (T)server;
        }

        public T CreateServer<T>(string selfIP, int selfPort) where T : UDPServer, new()
        {
            if (server != null)
            {
                MLog.Print("服务端只能有唯一一个，不能多次创建", MLogType.Warning);
                return null;
            }
            server = new T();
            server.Init(selfIP, selfPort);

            return (T)server;
        }

        public T CreateServer<T>(IPEndPoint selfEP) where T : UDPServer, new()
        {
            if (server != null)
            {
                MLog.Print("服务端只能有唯一一个，不能多次创建", MLogType.Warning);
                return null;
            }
            server = new T();
            server.Init(selfEP);

            return (T)server;
        }

        public T CreateClient<T>(string serverIP, int serverPort, float interval = 5.0f, bool enableThread = true) where T : UDPClient, new()
        {
            var client = new T();
            client.Init(serverIP, serverPort, interval, enableThread);
            clients.Add(client);
            return client;
        }
    }
}