using System;
using System.Net;
using System.Net.Sockets;

namespace MFramework
{
    public class SendContextBase
    {
        public UInt16 Type { get; set; }
        public byte[] Buff { get; set; }
    }

    public class UDPSendContext : SendContextBase
    {
        public EndPoint EndPoint { get; set; }
    }

    public class TCPSendContext : SendContextBase
    {
        public Socket Socket { get; set; }
    }
}
