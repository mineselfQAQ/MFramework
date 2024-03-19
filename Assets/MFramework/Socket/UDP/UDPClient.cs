using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;

namespace MFramework
{
    public class UDPClient
    {
        private Socket socket;

        private byte[] sendData;

        private EndPoint endEP;

        public UDPClient(string serverIP, int serverPort)
        {
            Init(serverIP, serverPort);
        }

        private void Init(string serverIP, int serverPort)
        {
            //Ipv4，使用的是数据报，也就是UDP
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            endEP = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
        }
        public void Quit()
        {
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

            socket.SendTo(sendData, sendData.Length, SocketFlags.None, endEP);
        }
    }
}