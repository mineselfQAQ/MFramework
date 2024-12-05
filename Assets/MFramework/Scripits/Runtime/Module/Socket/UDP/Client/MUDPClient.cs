using System.Net.Sockets;
using System.Net;
using System;
using Codice.Client.BaseCommands;

namespace MFramework
{
    public class MUDPClient
    {
        public string IP;//������IP
        public int Port;//������Port

        private Socket _client;
        private EndPoint _endPoint;//��������ַ
        private DataBuffer _dataBuffer = new DataBuffer();

        public MUDPClient(string ip, int port)
        {
            //��������������
            IP = ip;
            Port = port;
            _endPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            _client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public void Connect()
        {
            try
            {
                _client.Bind(new IPEndPoint(IPAddress.Any, 0));//���Լ�
                _client.Connect(_endPoint);//���ӷ�����
            }
            catch (SocketException e)
            {
                throw new Exception("��ʧ��");
            }
            //�������������֤
            byte[] array = new byte[1] { 1 };
            //TODO:�ĳ�������Send()
            _client.Send(array);
            array = new byte[1];
            _client.ReceiveTimeout = 5000;//��������
            int receivedUdp = _client.Receive(array);
            _client.ReceiveTimeout = 0;//�رս���
            if (receivedUdp != 1 || array[0] != 1)
            {
                throw new Exception("������֤ʧ��");
            }
            MLog.Print($"{typeof(MUDPClient)}���ͻ�����������������<{_endPoint}>");

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
                    //MainThreadUtility.Post<SocketDataPack>(onTrigger, dataPack);
                    //MainThreadUtility.Post<SocketDataPack>(OnSend, dataPack);
                }), _client);
            }
            catch (SocketException ex)
            {
                //������ȥ�����жϿ�������
                //OnErrorInternal(ex);
            }
        }

        private void ReceiveData()
        {
            byte[] bytes = new byte[8 * 1024];//��������С
            _client.BeginReceive(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(OnReceiveData), bytes);
        }
        private void OnReceiveData(IAsyncResult result)
        {
            try
            {
                byte[] buffer = (byte[])result.AsyncState;
                int len = _client.EndReceive(result);



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
