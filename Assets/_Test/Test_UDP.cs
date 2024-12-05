using MFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_UDP : MonoBehaviour
{
    public string ip;
    public int port;

    private MUDPServer server;
    private MUDPClient client;

    private void Start()
    {

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
            client.Send((System.UInt16)SocketEvent.test, bytes);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            server.Quit();
        }
    }
}
