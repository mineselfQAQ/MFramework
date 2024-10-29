namespace MFramework
{
    public enum SocketEvent
    {
        //Client--->Server
        cs_head = 0x0001,//心跳包
        cs_disconnect = 0x0002,//客户端主动断开
        //Server--->Client
        sc_kickout = 0x0003,//服务端踢出

        test = 0x1001,//测试用
    }
}