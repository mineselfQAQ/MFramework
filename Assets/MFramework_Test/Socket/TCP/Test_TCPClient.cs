using MFramework;
using UnityEngine;

public class Test_TCPClient : MonoBehaviour
{
    MTCPClient _client;
    private void Awake()
    {
        _client = new MTCPClient("127.0.0.1", 6854);
        _client.OnDisconnect += () =>
        {
            UnityEngine.Debug.Log("�Ͽ�����");
        };

        _client.OnReceive += (dataPack) =>
        {
            UnityEngine.Debug.LogFormat("��������>>>{0}", (SocketEvent)dataPack.Type);
        };
        _client.OnSend += (dataPack) =>
        {
            UnityEngine.Debug.LogFormat("��������>>>{0}", (SocketEvent)dataPack.Type);
        };
        _client.OnError += (ex) =>
        {
            UnityEngine.Debug.LogFormat("�����쳣>>>{0}", ex);
        };

        _client.OnReConnectSuccess += (num) =>
        {
            UnityEngine.Debug.LogFormat("��{0}�������ɹ�", num);
        };
        _client.OnReConnectError += (num) =>
        {
            UnityEngine.Debug.LogFormat("��{0}������ʧ��", num);
        };
        _client.OnReconnecting += (num) =>
        {
            UnityEngine.Debug.LogFormat("���ڽ��е�{0}������", num);
        };

        MCoroutineManager.Instance.DelayNoRecord(() =>
        {
            _client.Connect(() =>
            {
                UnityEngine.Debug.Log("���ӳɹ�");
            }, () =>
            {
                UnityEngine.Debug.Log("����ʧ��");
            });
        }, 1.0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _client.Connect(() =>
            {
                UnityEngine.Debug.Log("���ӳɹ�");
            }, () =>
            {
                UnityEngine.Debug.Log("����ʧ��");
            });
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            byte[] bytes = new byte[1024*1024];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = 1;
            }
            _client.SendBytes(SocketEvent.EMPTY, bytes);
        }
    }

    public void ClickSendTest()
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes("���ǲ�������");
        _client.SendBytes(SocketEvent.EMPTY, bytes);
    }

    public void ClickDisConnect()
    {
        _client.Disconnect();
    }

    private void OnApplicationQuit()
    {
        // ע������Unity�����������£���Ϸ����/�ر�ֻӰ�����̵߳Ŀ��أ���Ϸ�رջص�ʱ��Ҫͨ��Close�������رշ����/�ͻ��˵��̡߳�
        if (_client != null)
        {
            _client.Close();
        }
    }
}
