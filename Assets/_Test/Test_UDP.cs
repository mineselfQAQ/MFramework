using MFramework;
using System.Collections;
using System.Collections.Generic;
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
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            client = new MUDPClient(ip, port);
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
        if (client != null)
        {
            client.Disconnect();
        }

        MCoroutineManager.Instance.DelayNoRecord(() =>
        {
            if (server != null)
            {
                server.Close();
            }
        }, 1.0f);
    }
}
