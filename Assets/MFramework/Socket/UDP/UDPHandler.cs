using System.Collections.Generic;
using System.Net;

namespace MFramework
{
    public class UDPHandler : MonoSingleton<UDPHandler>
    {
        private UDPClient Default;

        private UDPServer server;
        private List<UDPClient> clients;

        private UDPHandler() 
        {
            Default = new UDPClient();
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

        /// <summary>
        /// 向服务器仅发送一条消息
        /// </summary>
        public void Send(string sendStr, EndPoint serverEP)
        {
            Default.Send(sendStr, serverEP);
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

        public T CreateClient<T>(string serverIP, int serverPort, bool enableThread = true) where T : UDPClient, new()
        {
            var client = new T();
            client.Init(serverIP, serverPort, enableThread);
            clients.Add(client);
            return client;
        }
    }
}