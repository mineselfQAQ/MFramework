using MFramework;

public class TestClient : UDPClient
{
    protected override void DoAfterReceive(string receiveStr)
    {

    }

    protected override void MainThreadDoAfterReceive(string receiveStr)
    {
        MLog.Print($"�յ�����Server����Ϣ��{receiveStr}");
    }
}
