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
        //ע�⣺
        //1.����������ֻ��Ҫ������Port��IP�����Զ���ȡ
        //2.�����ͻ����ṩ��IP��Port�Ƿ������Ķ����������
        //3.����������ٿͻ���֮ǰ����(�Ѵ���)
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