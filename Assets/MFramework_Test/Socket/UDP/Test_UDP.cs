using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MFramework;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public class Test_UDP : MonoBehaviour
{
    public Button btn;
    public TMP_InputField inputField;
    public TMP_Text text;

    private UDPClient client;
    private TestServer server;

    private void Start()
    {
        Debug.Log(GetIP());

        client = new UDPClient("192.168.1.7", 8080);
        server = new TestServer("192.168.1.7", 8080);

        btn.onClick.AddListener(() =>
        {
            client.Send(inputField.text);
        });
    }

    private void Update()
    {
        MainThreadSynchronizationContext.Instance.Update();//不要忘了添加

        text.text = server.ReceiveStr;
    }

    private void OnApplicationQuit()
    {
        server.Quit();
        client.Quit();
    }

    public string GetIP()
    {
        string output = "";

        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
            NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;  //无线局域网适配器 

            if ((item.NetworkInterfaceType == _type1) && item.OperationalStatus == OperationalStatus.Up)
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        output = ip.Address.ToString();
                    }
                }
            }
        }
        return output;
    }
}