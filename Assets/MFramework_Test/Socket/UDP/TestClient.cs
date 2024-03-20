using MFramework;

public class TestClient : UDPClient
{
    protected override void DoAfterReceive(string receiveStr)
    {

    }

    protected override void MainThreadDoAfterReceive(string receiveStr)
    {
        MLog.Print($"收到来自Server的消息：{receiveStr}");
    }
}
