using System.Net;

namespace MFramework
{
    //Tip：必须是class，引用类型才能简单赋值，否则类似info.HeadTime = time;没有ref无法赋值
    public class UDPClientSocketInfo
    {
        public EndPoint Client;//所属客户端
        public DataBuffer DataBuffer;//数据缓存
        public long HeadTime;//心跳包时间戳
    }
}