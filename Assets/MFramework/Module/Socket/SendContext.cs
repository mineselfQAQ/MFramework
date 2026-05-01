using System.Net;
using System.Net.Sockets;
using NetSocket = System.Net.Sockets.Socket;

namespace MFramework.Socket
{
    public class SendContextBase
    {
        public ushort Type { get; set; }
        public byte[] Buffer { get; set; }
    }

    public sealed class UDPSendContext : SendContextBase
    {
        public EndPoint EndPoint { get; set; }
    }

    public sealed class TCPSendContext : SendContextBase
    {
        public NetSocket Socket { get; set; }
    }
}
