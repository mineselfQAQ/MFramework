using MFramework;
using UnityEngine;

public class Test_TCPServer : MonoBehaviour
{
    MTCPServer _server;

    private void Awake()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            _server = new MTCPServer("127.0.0.1", 6854);

            _server.OnConnect += (client) =>
            {
                UnityEngine.Debug.LogFormat("连接成功 >> IP:{0}", client.LocalEndPoint.ToString());
            };
            _server.OnDisconnect += (client) =>
            {
                UnityEngine.Debug.LogFormat("连接断开 >> IP:{0}", client.LocalEndPoint.ToString());
            };
            _server.OnReceive += (client, data) =>
            {
                UnityEngine.Debug.LogFormat("[{0}]接收到数据>>>{1} {2}", client.LocalEndPoint.ToString(), (SocketEvent)data.Type, data.Buff.Length);

                switch ((SocketEvent)data.Type)
                {
                    case SocketEvent.empty:
                        UnityEngine.Debug.LogFormat("接收到测试数据 >>> {0}", System.Text.Encoding.UTF8.GetString(data.Data));
                        break;
                }
            };
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            // 踢出连接
            foreach (var item in _server.ClientInfoDic.Keys)
            {
                _server.KickOutAll();
            }
        }
    }

    private void OnApplicationQuit()
    {
        // 注意由于Unity编译器环境下，游戏开启/关闭只影响主线程的开关，游戏关闭回调时需要通过Close函数来关闭服务端/客户端的线程。
        if (_server != null)
        {
            _server.Close();
        }
    }
}
