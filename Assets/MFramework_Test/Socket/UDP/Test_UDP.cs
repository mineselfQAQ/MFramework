using MFramework;
using System.Threading;
using UnityEngine;

public class Test_UDP : MonoBehaviour
{
    public bool AutoGetIP;
    public string ip;
    public int port;

    private MUDPServer server;
    private MUDPClient client;

    private void Start()
    {
        if(AutoGetIP) ip = MSocketUtility.GetDefaultNICIPV4Address().ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            server = new MUDPServer(ip, port);
            server.OnReceive += (ep, dataPack) =>
            {
                MLog.Print($"服务器：收到来自客户端<{ep}>的消息：{MConvertUtility.BytesToUTF8(dataPack.Data)}");
            };
            server.Open();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            client = new MUDPClient(ip, port);
            client.OnDisconnect += () =>
            {
                MLog.Print("客户端：已断开连接");
            };
            client.Connect();
        }

        var bytes = System.Text.Encoding.UTF8.GetBytes("我是测试数据");
        if (Input.GetKeyDown(KeyCode.X))
        {
            client.SendBytes(SocketEvent.EMPTY, bytes);
            client.SendASCII(SocketEvent.EMPTY, "ABC");
            client.SendUTF(SocketEvent.EMPTY, "你好");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            server.Close();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            client.Disconnect();
        }
    }

    private void OnApplicationQuit()
    {
        if (server != null && server.isValid)
        {
            //退出方法：由服务器申请关闭所有客户端后再关闭
            Thread thread = new Thread(() =>
            {
                server.Close();
            });
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
