using MFramework;

public class TestServer : UDPServer
{
    protected override void DoAfterReceive(string receiveStr)
    {
        Send("���Ƿ�����");
    }

    protected override void MainThreadDoAfterReceive(string receiveStr)
    {
        MLog.Print($"�յ�����Client����Ϣ��{receiveStr}");
    }
}
