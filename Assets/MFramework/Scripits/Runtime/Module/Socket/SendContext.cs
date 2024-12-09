using System;
using System.Net;

namespace MFramework
{
    public class SendContext
    {
        public EndPoint EndPoint { get; set; }
        public UInt16 Type { get; set; }
        public byte[] Buff { get; set; }
    }
}
