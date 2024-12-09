namespace MFramework
{
    public enum SocketEvent
    {
        empty = 0x0000,//测试用

        //Client--->Server
        cs_head = 0x0001,//心跳包
        cs_disconnect = 0x0002,//客户端请求断开
        //Server--->Client
        sc_kickout = 0x0010,//服务端踢出
        sc_disconnect = 0x0011,//服务端断开回复
    }
}