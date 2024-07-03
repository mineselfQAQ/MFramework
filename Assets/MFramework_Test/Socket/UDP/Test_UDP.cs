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
        //1.创建服务器只需要服务器Port，IP可以自动获取
        //2.创建客户端提供的IP和Port是服务器的而不是自身的
        //3.服务器最好再客户端之前创建(已处理)
        client = UDPManager.Instance.CreateClient<TestClient>("192.168.50.12", 8080, 5.0f, true);

        btn.onClick.AddListener(() =>
        {
            client.Send(inputField.text);
        });
    }

    private void Update()
    {
        if(server != null) text.text = server.ReceiveStr;

        if (Input.GetKeyDown(KeyCode.E))
        {
            server = UDPManager.Instance.CreateServer<TestServer>(8080);
        }
    }
}