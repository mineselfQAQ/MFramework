using System.Net;

namespace MFramework.Socket
{
    public interface IMSocketFactory
    {
        MTCPClient CreateTCPClient(string ip, int port);
        MTCPServer CreateTCPServer(string ip, int port);
        MUDPClient CreateUDPClient(string ip, int port);
        MUDPClient CreateUDPClient(IPEndPoint endPoint);
        MUDPServer CreateUDPServer(string ip, int port);
        MUDPServer CreateUDPServer(IPEndPoint endPoint);
    }

    public sealed class MSocketFactory : IMSocketFactory
    {
        private readonly ISocketMainThreadDispatcher _dispatcher;

        public MSocketFactory(ISocketMainThreadDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public MTCPClient CreateTCPClient(string ip, int port)
        {
            return new MTCPClient(ip, port, _dispatcher);
        }

        public MTCPServer CreateTCPServer(string ip, int port)
        {
            return new MTCPServer(ip, port, _dispatcher);
        }

        public MUDPClient CreateUDPClient(string ip, int port)
        {
            return new MUDPClient(ip, port, _dispatcher);
        }

        public MUDPClient CreateUDPClient(IPEndPoint endPoint)
        {
            return new MUDPClient(endPoint, _dispatcher);
        }

        public MUDPServer CreateUDPServer(string ip, int port)
        {
            return new MUDPServer(ip, port, _dispatcher);
        }

        public MUDPServer CreateUDPServer(IPEndPoint endPoint)
        {
            return new MUDPServer(endPoint, _dispatcher);
        }
    }
}
