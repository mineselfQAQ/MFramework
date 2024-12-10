namespace MFramework
{
    public enum SocketEvent
    {
        EMPTY = 0x0000,//测试用

        //Client--->Server
        C2S_HEAD = 0x0001,//心跳包
        C2S_DISCONNECTREQUEST = 0x0002,//客户端请求断开
        C2S_DISCONNECTREPLY = 0x0003,//客户端断开后回复
        //Server--->Client
        S2C_KICKOUT = 0x0010,//服务端踢出
        S2C_DISCONNECTREPLY = 0x0011,//服务端断开客户端后回复
        S2C_DISCONNECTREQUEST = 0x0012,//服务端请求断开客户端
    }
}