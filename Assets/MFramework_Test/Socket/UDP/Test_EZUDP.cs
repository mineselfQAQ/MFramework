using MFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_EZUDP : MonoBehaviour
{
    public string ip;
    public int port;

    private MEzUDPServer server;
    private MEzUDPClient client;

    void Start()
    {
        server = new MEzUDPServer(ip, port);
        client = new MEzUDPClient(ip, port);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            byte[] bytes = new byte[65507];//最大容量(65535 - 20(IP头) - 8(UDP头))，再+1则超出会丢弃
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = 1;
            }
            client.SendBytes(bytes);
        }
    }
}
