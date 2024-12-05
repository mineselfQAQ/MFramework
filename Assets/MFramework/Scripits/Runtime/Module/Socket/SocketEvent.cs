namespace MFramework
{
    public enum SocketEvent
    {
        //Client--->Server
        cs_head = 0x0001,//心跳包
        cs_disconnect = 0x0002,//客户端主动断开
        cs_connect = 0x0003,//连接包(客户端向服务器发起)
        //Server--->Client
        sc_kickout = 0x0010,//服务端踢出
        sc_connect = 0x0011,//服务端踢出(服务器向客户端回应)

        test = 0x1001,//测试用
    }
}