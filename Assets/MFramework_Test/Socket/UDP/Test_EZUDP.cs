using MFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_EZUDP : MonoBehaviour
{
    public string ip;
    public int port;

    private MEzUDPServer server;

    void Start()
    {
        server = new MEzUDPServer(ip, port);
    }

    void Update()
    {
        
    }
}
