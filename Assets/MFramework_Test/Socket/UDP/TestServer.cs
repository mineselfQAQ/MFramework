using MFramework;

public class TestServer : UDPServer
{
    public TestServer(string selfIP, int selfPort) : base(selfIP, selfPort)
    {

    }

    protected override void DoAfterReceive(string receiveStr)
    {
        MLog.Print(receiveStr);
    }
}
