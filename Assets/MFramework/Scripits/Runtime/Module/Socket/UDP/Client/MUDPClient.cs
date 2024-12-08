using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine;

namespace MFramework
{
    public class MUDPClient
    {
        public bool isConnect { get; private set; }

        private Socket _client;
        private EndPoint _serverEP;//��������ַ
        private DataBuffer _dataBuffer = new DataBuffer();

        public MUDPClient(string ip, int port)
        {
            //��������������
            var ep = new IPEndPoint(IPAddress.Parse(ip), port);

            InitSettings(ep);
        }
        public MUDPClient(IPEndPoint ep)
        {
            InitSettings(ep);
        }

        public void InitSettings(IPEndPoint ep)
        {
            _serverEP = ep;

            _client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            isConnect = false;
        }

        public void Connect()
        {
            if (isConnect) MLog.Print($"{typeof(MUDPClient)}�������������������������𷴸�����", MLogType.Warning);

            try
            {
                _client.Bind(new IPEndPoint(IPAddress.Any, 0));//���Լ�
                _client.Connect(_serverEP);//���ӷ�����
            }
            catch (SocketException e)
            {
                throw new Exception($"��ʧ�ܣ�{e}");
            }
            //�������������֤
            byte[] buff = new byte[4] { 18, 203, 59, 38 };//�����ĸ���
            _client.Send(buff);
            buff = new byte[1];
            _client.ReceiveTimeout = 5000;//��������
            int len = _client.Receive(buff);
            _client.ReceiveTimeout = 0;//�رս���
            if (len != 1 || buff[0] != 1)
            {
                MLog.Print("������֤ʧ��", MLogType.Error);
            }
            MLog.Print($"{typeof(MUDPClient)}���ͻ�����������������<{_serverEP}>");
            isConnect = true;

            ReceiveData();//�������ݽ���
        }

        public void Send(UInt16 e, byte[] buff = null)
        {
            //��ɰ���ȡ��Buff
            buff = buff ?? new byte[] { };
            var dataPack = new SocketDataPack(e, buff);
            var data = dataPack.Buff;

            try
            {
                //����Buff
                _client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback((asyncSend) =>
                {
                    Socket c = (Socket)asyncSend.AsyncState;
                    c.EndSend(asyncSend);
                }), _client);
            }
            catch (SocketException ex)
            {
                MLog.Print(ex);
            }
        }

        private void ReceiveData()
        {
            //Tip��Socket���������в������(ճ��ͨ��������)������Ҫ���ǲ���
            byte[] bytes = new byte[8 * 1024];//��������С
            _client.BeginReceive(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(OnReceiveData), bytes);
        }
        private void OnReceiveData(IAsyncResult result)
        {
            try
            {
                byte[] bytes = (byte[])result.AsyncState;
                int len = _client.EndReceive(result);
                if (len > 0)
                {
                    //���ݼ��뻺������(���ݿ��ܷ�������Ҳ����ͬʱ������)
                    _dataBuffer.AddBuffer(bytes, len);
                    //��ȡ����(�����ȡ)
                    var dataPack = new SocketDataPack();
                    if (_dataBuffer.TryUnpack(out dataPack))
                    {
                        //if (dataPack.Type == (UInt16)SocketEvent.xxx)
                        //{

                        //}
                        
                        Debug.Log($"�յ����Է�����<{_serverEP}>����Ϣ��{dataPack.ToString()}");
                    }
                }

                //������������
                ReceiveData();
            }
            catch (Exception e)
            {
                MLog.Print("���ݽ���ʧ�ܣ�" + e.Message, MLogType.Warning);
            }
        }
    }
}
