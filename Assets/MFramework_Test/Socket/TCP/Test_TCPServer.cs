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
                UnityEngine.Debug.LogFormat("���ӳɹ� >> IP:{0}", client.LocalEndPoint.ToString());
            };
            _server.OnDisconnect += (client) =>
            {
                UnityEngine.Debug.LogFormat("���ӶϿ� >> IP:{0}", client.LocalEndPoint.ToString());
            };
            _server.OnReceive += (client, data) =>
            {
                UnityEngine.Debug.LogFormat("[{0}]���յ�����>>>{1} {2}", client.LocalEndPoint.ToString(), (SocketEvent)data.Type, data.Buff.Length);

                switch ((SocketEvent)data.Type)
                {
                    case SocketEvent.empty:
                        UnityEngine.Debug.LogFormat("���յ��������� >>> {0}", System.Text.Encoding.UTF8.GetString(data.Data));
                        break;
                }
            };
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            // �߳�����
            foreach (var item in _server.ClientInfoDic.Keys)
            {
                _server.KickOutAll();
            }
        }
    }

    private void OnApplicationQuit()
    {
        // ע������Unity�����������£���Ϸ����/�ر�ֻӰ�����̵߳Ŀ��أ���Ϸ�رջص�ʱ��Ҫͨ��Close�������رշ����/�ͻ��˵��̡߳�
        if (_server != null)
        {
            _server.Close();
        }
    }
}
