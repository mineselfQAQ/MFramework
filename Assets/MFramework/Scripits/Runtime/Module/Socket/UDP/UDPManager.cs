using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MFramework
{
    public class UDPManager : MonoSingleton<UDPManager>
    {
        private UDPServer server;
        private List<UDPClient> clients;

        #region StaticMethod
        /// <summary>
        /// �������������һ����Ϣ
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
        /// ����������Ͳ�������Ϣ
        /// </summary>
        public static string SendAndReceive(string sendStr, IPEndPoint serverEP)
        {
            using (UdpClient udpClient = new UdpClient())
            {
                try
                {
                    //������ʱ��ȴ�1�룬����ʱ(Tip:���ͨ����������ʱ���Բ��ƣ�1�뻹û�յ���Ȼ��������)
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

        private UDPManager() 
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
                MLog.Print($"{typeof(UDPManager)}�������ֻ����Ψһһ�������ܶ�δ���������", MLogType.Warning);
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
                MLog.Print($"{typeof(UDPManager)}�������ֻ����Ψһһ�������ܶ�δ���������", MLogType.Warning);
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
                MLog.Print($"{typeof(UDPManager)}�������ֻ����Ψһһ�������ܶ�δ���������", MLogType.Warning);
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