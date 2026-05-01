using System.Net.Sockets;
using System.Threading;
using NetSocket = System.Net.Sockets.Socket;

namespace MFramework.Socket
{
    public sealed class TCPClientSocketInfo
    {
        public NetSocket Client;
        public Thread ReceiveThread;
        public DataBuffer DataBuffer;
        public long HeadTime;
    }
}
