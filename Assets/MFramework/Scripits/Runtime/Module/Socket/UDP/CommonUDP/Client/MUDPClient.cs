using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using System.Timers;
using UnityEngine;

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

        //=====����=====
        public void Connect(Action onSuccess = null, Action onError = null)
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
                    MainThreadUtility.Post(onSuccess);
                    MainThreadUtility.Post(OnConnectSuccess);
                }
                else
                {
                    MLog.Print($"{typeof(MUDPClient)}���ͻ���������������<{serverEP}>ʧ�ܣ�{ex}", MLogType.Warning);

                    MainThreadUtility.Post(onError);
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
                _client.Connect(serverEP);//���ӷ�����(�����������ӣ�Ϊ�󶨷�����IP)
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
                //���ӳɹ�
                onTrigger(true, null);
                MLog.Print($"{typeof(MUDPClient)}���ͻ�����������������<{serverEP}>");
                isConnect = true;

                //��ʱ����������
                _headTimer = new System.Timers.Timer(HEAD_OFFSET);
                _headTimer.AutoReset = true;
                _headTimer.Elapsed += delegate (object sender, ElapsedEventArgs args)
                {
                    SendEvent(SocketEvent.C2S_HEAD);
                };
                _headTimer.Start();

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
        private void ReConnect(int num, int index, float reconnectDelay = 1.0f)
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
                //�ӳ�����
                MCoroutineManager.Instance.DelayNoRecord(() =>
                {
                    MainThreadUtility.Post<int>(OnReConnectError, index);
                    ReConnect(num, index);//ʧ���ٴ�����
                }, reconnectDelay);
            });
        }



        //=====����=====
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
                        //�ر�/�߳���
                        if (dataPack.Type == (UInt16)SocketEvent.S2C_DISCONNECT ||
                            dataPack.Type == (UInt16)SocketEvent.S2C_KICKOUT)
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
                if (isConnect) ReceiveData();
            }
            catch (Exception ex)
            {
                OnErrorInternal(ex);
            }
        }



        //=====����=====
        public void SendUTF(SocketEvent type, string message, Action<SocketDataPack> onTrigger = null)
        {
            byte[] buff = Encoding.UTF8.GetBytes(message);
            UDPSendContext context = new UDPSendContext() {Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendASCII(SocketEvent type, string message, Action<SocketDataPack> onTrigger = null)
        {
            byte[] buff = Encoding.ASCII.GetBytes(message);
            UDPSendContext context = new UDPSendContext() {Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendBytes(SocketEvent type, byte[] buff, Action<SocketDataPack> onTrigger = null)
        {
            UDPSendContext context = new UDPSendContext() {Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendEvent(SocketEvent type, Action<SocketDataPack> onTrigger = null)
        {
            UDPSendContext context = new UDPSendContext() { Type = (ushort)type, Buff = null };

            Send(context, onTrigger);
        }
        protected override void Send(UDPSendContext context, Action<SocketDataPack> onTrigger)
        {
            if (!isConnect) return;

            //��ɰ���ȡ��Buff
            context.Buff = context.Buff ?? new byte[] { };
            var dataPack = new SocketDataPack(context.Type, context.Buff);
            var data = dataPack.Buff;

            //����Buff
            //Tip�������ǲ��ᱨ��ģ���������ش����Ʊ��⴫��ʧ��
            //TODO:����Ӧ���������ģ���ͷ�������ACK���ƣ����ڴ˿̿�ʼ��ʱ������ڹ涨ʱ�����յ�һ��ACK����ô����δ��ʱ
            _client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback((asyncSend) =>
            {
                Socket c = (Socket)asyncSend.AsyncState;
                c.EndSend(asyncSend);

                MainThreadUtility.Post<SocketDataPack>(onTrigger, dataPack);
                MainThreadUtility.Post<SocketDataPack>(OnSend, dataPack);
            }), _client);
        }
        protected override void Send(UDPSendContext context, Action<byte[]> onTrigger)
        {
            throw new NotSupportedException();
        }

        //=====����=====
        /// <summary>
        /// ����ر�����(�յ��رհ�����ʽ�ر�)
        /// </summary>
        public void Disconnect()
        {
            if (!isConnect)
            {
                MLog.Print($"{typeof(MUDPClient)}:�ͻ����ѶϿ����ӣ������ظ�����", MLogType.Warning);
                return;
            }

            SendEvent(SocketEvent.C2S_DISCONNECT);
        }

        private void DisconnectInternal()
        {
            MLog.Print($"{typeof(MUDPClient)}���ͻ����յ����Է�����<{serverEP}>�Ĺرջظ����ͻ��������ر�");

            Close();
            MainThreadUtility.Post(OnDisconnect);
        }

        protected override void OnCloseInternal()
        {
            if (!isConnect) return;
            isConnect = false;

            OnConnectSuccess = null;
            OnConnectError = null;
            OnReConnectSuccess = null;
            OnReConnectError = null;
            OnReconnecting = null;
            OnDisconnect = null;
            OnReceive = null;
            OnSend = null;
            OnError = null;

            if (_headTimer != null)
            {
                _headTimer.Stop();
                _headTimer = null;
            }
            if (_connTimeoutTimer != null)
            {
                _connTimeoutTimer.Stop();
                _connTimeoutTimer = null;
            }
        }

        //=====�ڲ��¼�=====
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
