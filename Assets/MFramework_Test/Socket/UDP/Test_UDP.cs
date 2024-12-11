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
                MLog.Print($"���������յ����Կͻ���<{ep}>����Ϣ��{MConvertUtility.BytesToUTF8(dataPack.Data)}");
            };
            server.Open();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            client = new MUDPClient(ip, port);
            client.OnDisconnect += () =>
            {
                MLog.Print("�ͻ��ˣ��ѶϿ�����");
            };
            client.Connect();
        }

        var bytes = System.Text.Encoding.UTF8.GetBytes("���ǲ�������");
        if (Input.GetKeyDown(KeyCode.X))
        {
            client.SendBytes(SocketEvent.EMPTY, bytes);
            client.SendASCII(SocketEvent.EMPTY, "ABC");
            client.SendUTF(SocketEvent.EMPTY, "���");
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
            //�˳��������ɷ���������ر����пͻ��˺��ٹر�
            Thread thread = new Thread(() =>
            {
                server.Close();
            });
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
