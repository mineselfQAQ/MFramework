using System.Net;

namespace MFramework.Socket
{
    public sealed class UDPClientSocketInfo
    {
        public EndPoint Client;
        public DataBuffer DataBuffer;
        public UDPDataPackAssembler Assembler;
        public long HeadTime;
    }
}
