using MFramework;

public class TestServer : UDPServer
{
    protected override void DoAfterReceive(string receiveStr)
    {
        Send("我是服务器");
    }

    protected override void MainThreadDoAfterReceive(string receiveStr)
    {
        MLog.Print($"收到来自Client的消息：{receiveStr}");
    }
}
