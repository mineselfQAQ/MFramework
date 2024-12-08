using System.Net.Sockets;
using System.Threading;

namespace MFramework
{
    //Tip：必须是class，引用类型才能简单赋值，否则类似info.HeadTime = time;没有ref无法赋值
    public class TCPClientSocketInfo
    {
        public Socket Client;//所属客户端
        public Thread ReceiveThread;//所属线程
        public long HeadTime;//心跳包时间戳
    }
}