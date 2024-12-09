using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine;
using System.Text;
using System.Timers;

namespace MFramework
{
    public class MUDPClient : MUDPClientBase
    {
        public event Action OnConnectSuccess;
        public event Action OnConnectError;
        public event Action<int> OnReConnectSuccess;
        public event Action<int> OnReConnectError;
        public event Action<int> OnReconnecting;
        public event Action OnDisconnect;
        public event Action<SocketDataPack> OnReceive;
        public event Action<SocketDataPack> OnSend;
        public event Action<Exception> OnError;

        private const int TIMEOUT_CONNECT = 3000;//���ӳ�ʱʱ��
        private const int TIMEOUT_SEND = 3000;//���ͳ�ʱʱ��
        private const int TIMEOUT_RECEIVE = 3000;//���ճ�ʱʱ��

        private const int HEAD_OFFSET = 2000;//���������ͼ��
        private const int RECONN_MAX_SUM = 3;//�����������

        private Timer _connTimeoutTimer;
        private Timer _headTimer;

        private DataBuffer _dataBuffer = new DataBuffer();

        public bool isConnect { get; private set; }
        public bool isConnecting { get; private set; }
        public bool isReconnecting { get; private set; }

        public MUDPClient(string ip, int port) : base(ip, port) { }
        public MUDPClient(IPEndPoint ep) : base(ep) { }

        public void Connect(Action success = null, Action error = null)
        {
            if (isConnecting) return;
            isConnecting = true;

            if (isConnect) MLog.Print($"{typeof(MUDPClient)}�������������������������𷴸�����", MLogType.Warning);

            Action<bool, string> onTrigger = (flag, ex) =>
            {
                isConnecting = false;

                //�ɹ���ʧ�ܻص�
                if (flag)
                {
                    MLog.Print($"{typeof(MUDPClient)}���ͻ�����������������<{_serverEP}>", MLogType.Warning);
                    isConnect = true;

                    MainThreadUtility.Post(success);
                    MainThreadUtility.Post(OnConnectSuccess);
                }
                else
                {
                    MLog.Print($"{typeof(MUDPClient)}���ͻ���������������<{_serverEP}>ʧ�ܣ�{ex}", MLogType.Warning);

                    MainThreadUtility.Post(error);
                    MainThreadUtility.Post(OnConnectError);
                }

                //�����TIMEOUT_CONNECT����ˣ�Ӧ�ùرռ�ʱ�����ⴥ��ʧ�ܻص�
                if (_connTimeoutTimer != null)
                {
                    _connTimeoutTimer.Stop();
                    _connTimeoutTimer = null;
                }
            };

            //TIMEOUT_CONNECT��û����ɣ�������ʧ��
            //Tip��ʧ�ܺ�Ӧ������ѡ�����(�ر�/����/...)
            _connTimeoutTimer = new Timer(TIMEOUT_CONNECT);
            _connTimeoutTimer.AutoReset = false;
            _connTimeoutTimer.Elapsed += delegate (object sender, ElapsedEventArgs args)
            {
                onTrigger(false, "���ӳ�ʱ");
            };

            try
            {
                _client.Bind(new IPEndPoint(IPAddress.Any, 0));//���Լ�
                _client.Connect(_serverEP);//���ӷ�����(�����������ӣ�Ϊ�󶨷�����IP)
                //�������������֤
                _connTimeoutTimer.Start();//��ʼ��ʱ
                byte[] buff = new byte[4] { 18, 203, 59, 38 };//�����ĸ���
                _client.Send(buff);
                buff = new byte[1];
                int len = _client.Receive(buff);
                if (len != 1 || buff[0] != 1)
                {
                    throw new Exception("������֤ʧ��");
                }
                onTrigger(true, null);

                ReceiveData();//�������ݽ���
            }
            catch (Exception e)
            {
                onTrigger(false, e.Message);
            }
        }

        public void ReConnect(int num = RECONN_MAX_SUM)
        {
            ReConnect(num, 0);
        }
        private void ReConnect(int num, int index)
        {
            isReconnecting = true;

            num--;
            index++;
            if (num < 0)
            {
                DisconnectInternal();
                isReconnecting = false;
                return;
            }

            MainThreadUtility.Post<int>(OnReconnecting, index);
            Connect(() =>
            {
                MainThreadUtility.Post<int>(OnReConnectSuccess, index);
                isReconnecting = false;
            }, () =>
            {
                MainThreadUtility.Post<int>(OnReConnectError, index);
                ReConnect(num, index);//ʧ���ٴ�����
            });
        }

        //����ر�����(�յ��رհ�����ʽ�ر�)
        public void Disconnect()
        {
            SendBytes(SocketEvent.cs_disconnect);
        }
        private void DisconnectInternal()
        {
            MLog.Print("�ͻ����������ر�", MLogType.Warning);

            Close();
            MainThreadUtility.Post(OnDisconnect);
        }
        public void Close()
        {
            if (!isConnect) return;
            isConnect = false;

            if (_client != null)
            {
                _client.Close();
                _client = null;
            }
        }

        public void SendUTF(SocketEvent type, string message = null, Action<SocketDataPack> onTrigger = null)
        {
            byte[] buff = Encoding.UTF8.GetBytes(message);
            SendContext context = new SendContext() {Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendASCII(SocketEvent type, string message = null, Action<SocketDataPack> onTrigger = null)
        {
            byte[] buff = Encoding.ASCII.GetBytes(message);
            SendContext context = new SendContext() {Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendBytes(SocketEvent type, byte[] buff = null, Action<SocketDataPack> onTrigger = null)
        {
            SendContext context = new SendContext() {Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        protected override void Send(SendContext context, Action<SocketDataPack> onTrigger)
        {
            //��ɰ���ȡ��Buff
            context.Buff = context.Buff ?? new byte[] { };
            var dataPack = new SocketDataPack(context.Type, context.Buff);
            var data = dataPack.Buff;

            //����Buff
            //Tip�������ǲ��ᱨ��ģ���������ش����Ʊ��⴫��ʧ��
            _client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback((asyncSend) =>
            {
                Socket c = (Socket)asyncSend.AsyncState;
                c.EndSend(asyncSend);

                MainThreadUtility.Post<SocketDataPack>(onTrigger, dataPack);
                MainThreadUtility.Post<SocketDataPack>(OnSend, dataPack);
            }), _client);
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
                        //�رհ�
                        if (dataPack.Type == (UInt16)SocketEvent.sc_disconnect)
                        {
                            DisconnectInternal();
                        }
                        else
                        {
                            MainThreadUtility.Post<SocketDataPack>(OnReceive, dataPack);
                        }
                    }
                }

                //������������
                if(isConnect) ReceiveData();
            }
            catch (Exception ex)
            {
                OnErrorInternal(ex);
            }
        }

        private void OnErrorInternal(Exception ex)
        {
            Close();

            MainThreadUtility.Post<Exception>(OnError, ex);

            if (!isReconnecting)
            {
                ReConnect();
            }
        }
    }
}
