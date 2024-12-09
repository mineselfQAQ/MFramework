namespace MFramework
{
    public enum SocketEvent
    {
        EMPTY = 0x0000,//测试用

        //Client--->Server
        C2S_HEAD = 0x0001,//心跳包
        C2S_DISCONNECT = 0x0002,//客户端请求断开
        //Server--->Client
        S2C_KICKOUT = 0x0010,//服务端踢出
        S2C_DISCONNECT = 0x0011,//服务端断开回复
    }
}