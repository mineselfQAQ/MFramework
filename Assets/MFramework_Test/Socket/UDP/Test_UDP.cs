using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MFramework;

public class Test_UDP : MonoBehaviour
{
    public Button btn;
    public TMP_InputField inputField;
    public TMP_Text text;

    private TestClient client;
    private TestServer server;

    private void Start()
    {
        //注意：
        //服务端需要绑定的是自己，接收谁的消息不重要
        //客户端是谁不重要，重要的是向谁发消息
        //那么两者都会指向**服务端地址**
        server = UDPHandler.Instance.CreateServer<TestServer>("192.168.50.12", 8080);
        client = UDPHandler.Instance.CreateClient<TestClient>("192.168.50.12", 8080, true);

        btn.onClick.AddListener(() =>
        {
            client.Send(inputField.text);
        });
    }

    private void Update()
    {
        text.text = server.ReceiveStr;
    }
}