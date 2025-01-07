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
            UnityEngine.Debug.Log("断开连接");
        };

        _client.OnReceive += (dataPack) =>
        {
            UnityEngine.Debug.LogFormat("接收数据>>>{0}", (SocketEvent)dataPack.Type);
        };
        _client.OnSend += (dataPack) =>
        {
            UnityEngine.Debug.LogFormat("发送数据>>>{0}", (SocketEvent)dataPack.Type);
        };
        _client.OnError += (ex) =>
        {
            UnityEngine.Debug.LogFormat("出现异常>>>{0}", ex);
        };

        _client.OnReConnectSuccess += (num) =>
        {
            UnityEngine.Debug.LogFormat("第{0}次重连成功", num);
        };
        _client.OnReConnectError += (num) =>
        {
            UnityEngine.Debug.LogFormat("第{0}次重连失败", num);
        };
        _client.OnReconnecting += (num) =>
        {
            UnityEngine.Debug.LogFormat("正在进行第{0}次重连", num);
        };

        MCoroutineManager.Instance.DelayNoRecord(() =>
        {
            _client.Connect(() =>
            {
                UnityEngine.Debug.Log("连接成功");
            }, () =>
            {
                UnityEngine.Debug.Log("连接失败");
            });
        }, 1.0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _client.Connect(() =>
            {
                UnityEngine.Debug.Log("连接成功");
            }, () =>
            {
                UnityEngine.Debug.Log("连接失败");
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
        var bytes = System.Text.Encoding.UTF8.GetBytes("我是测试数据");
        _client.SendBytes(SocketEvent.EMPTY, bytes);
    }

    public void ClickDisConnect()
    {
        _client.Disconnect();
    }

    private void OnApplicationQuit()
    {
        // 注意由于Unity编译器环境下，游戏开启/关闭只影响主线程的开关，游戏关闭回调时需要通过Close函数来关闭服务端/客户端的线程。
        if (_client != null)
        {
            _client.Close();
        }
    }
}
