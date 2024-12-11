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
            byte[] bytes = new byte[65507];//�������(65535 - 20(IPͷ) - 8(UDPͷ))����+1�򳬳��ᶪ��
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = 1;
            }
            client.SendBytes(bytes);
        }
    }
}
