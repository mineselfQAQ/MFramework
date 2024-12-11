using System.Net.Sockets;
using System.Net;
using System;
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
        public event Action<UDPDataPack> OnReceive;
        public event Action<UDPDataPack> OnSend;
        public event Action<Exception> OnError;

        private const int TIMEOUT_CONNECT = 3000;//连接超时时间
        private const int HEAD_OFFSET = 2000;//心跳包发送间隔
        private const int RECONN_MAX_SUM = 3;//最大重连次数

        private Timer _connTimeoutTimer;
        private Timer _headTimer;

        private DataBuffer _dataBuffer = new DataBuffer();

        public bool isConnect { get; private set; }
        public bool isConnecting { get; private set; }
        public bool isReconnecting { get; private set; }

        public MUDPClient(string ip, int port) : base(ip, port) { }
        public MUDPClient(IPEndPoint ep) : base(ep) { }

        //=====连接=====
        public void Connect(Action onSuccess = null, Action onError = null)
        {
            if (isConnect) MLog.Print($"{typeof(MUDPClient)}：本机已连接至服务器，请勿反复连接", MLogType.Warning);

            if (isConnecting) return;
            isConnecting = true;

            Action<bool, string> onTrigger = (flag, ex) =>
            {
                isConnecting = false;

                //成功或失败回调
                if (flag)
                {
                    MainThreadUtility.Post(onSuccess);
                    MainThreadUtility.Post(OnConnectSuccess);
                }
                else
                {
                    MLog.Print($"{typeof(MUDPClient)}：客户端连接至服务器<{serverEP}>失败：{ex}", MLogType.Warning);

                    MainThreadUtility.Post(onError);
                    MainThreadUtility.Post(OnConnectError);
                }

                //如果在TIMEOUT_CONNECT完成了，应该关闭计时器避免触发失败回调
                if (_connTimeoutTimer != null)
                {
                    _connTimeoutTimer.Stop();
                    _connTimeoutTimer = null;
                }
            };

            //TIMEOUT_CONNECT内没有完成，则连接失败
            //Tip：失败后应该自主选择操作(关闭/重连/...)
            _connTimeoutTimer = new Timer(TIMEOUT_CONNECT);
            _connTimeoutTimer.AutoReset = false;
            _connTimeoutTimer.Elapsed += delegate (object sender, ElapsedEventArgs args)
            {
                onTrigger(false, "连接超时");
            };

            try
            {
                _client.Bind(new IPEndPoint(IPAddress.Any, 0));//绑定自己
                _client.Connect(serverEP);//连接服务器(并非真正连接，为绑定服务器IP)
                //向服务器发送验证
                _connTimeoutTimer.Start();//开始计时
                byte[] buff = new byte[4] { 18, 203, 59, 38 };//任意四个数
                _client.Send(buff);
                buff = new byte[1];
                int len = _client.Receive(buff);
                if (len != 1 || buff[0] != 1)
                {
                    throw new Exception("连接验证失败");
                }
                //连接成功
                onTrigger(true, null);
                MLog.Print($"{typeof(MUDPClient)}：客户端已连接至服务器<{serverEP}>");
                isConnect = true;

                //定时发送心跳包
                _headTimer = new System.Timers.Timer(HEAD_OFFSET);
                _headTimer.AutoReset = true;
                _headTimer.Elapsed += delegate (object sender, ElapsedEventArgs args)
                {
                    SendEvent(SocketEvent.C2S_HEAD);
                };
                _headTimer.Start();

                ReceiveData();//开启数据接收
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
                //延迟重连
                MCoroutineManager.Instance.DelayNoRecord(() =>
                {
                    MainThreadUtility.Post<int>(OnReConnectError, index);
                    ReConnect(num, index);//失败再次重连
                }, reconnectDelay);
            });
        }



        //=====接收=====
        private void ReceiveData()
        {
            //TODO:缺少重传机制，即使为1024也会出现数据丢失情况而且无法真正获取整包
            //1024---小于链路层负载(1472)的值
            byte[] bytes = new byte[1024];//缓冲区大小
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
                    //数据加入缓存器中(数据可能分批到达也可能同时到达多个)
                    _dataBuffer.AddBuffer(bytes, len);
                    //获取数据(解包获取)
                    TryUnpack();
                }

                //继续接收数据
                if (isConnect) ReceiveData();
            }
            catch (SocketException)
            {
                //OnErrorInternal(ex);
            }
        }

        private void TryUnpack()
        {
            //迭代解包所有包(网络问题导致的积压)
            while (_dataBuffer.haveBuff)
            {
                var dataPack = new UDPDataPack();
                if (_dataBuffer.TryUnpack(out dataPack))
                {
                    //关闭/踢出包
                    if (dataPack.Type == (UInt16)SocketEvent.S2C_DISCONNECTREPLY ||
                        dataPack.Type == (UInt16)SocketEvent.S2C_KICKOUT)
                    {
                        DisconnectInternal();
                    }
                    //关闭包(服务器请求)
                    else if (dataPack.Type == (UInt16)SocketEvent.S2C_DISCONNECTREQUEST)
                    {
                        SendEvent(SocketEvent.C2S_DISCONNECTREPLY);
                        MCoroutineManager.Instance.DelayOneFrame(() =>
                        {
                            DisconnectInternal();
                        });
                    }
                    else
                    {
                        MainThreadUtility.Post<UDPDataPack>(OnReceive, dataPack);
                    }
                }
            }
        }



        //=====发送=====
        public void SendUTF(SocketEvent type, string message, Action<UDPDataPack> onTrigger = null)
        {
            byte[] buff = Encoding.UTF8.GetBytes(message);
            UDPSendContext context = new UDPSendContext() {Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendASCII(SocketEvent type, string message, Action<UDPDataPack> onTrigger = null)
        {
            byte[] buff = Encoding.ASCII.GetBytes(message);
            UDPSendContext context = new UDPSendContext() {Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendBytes(SocketEvent type, byte[] buff, Action<UDPDataPack> onTrigger = null)
        {
            UDPSendContext context = new UDPSendContext() {Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendEvent(SocketEvent type, Action<UDPDataPack> onTrigger = null)
        {
            UDPSendContext context = new UDPSendContext() { Type = (ushort)type, Buff = null };

            Send(context, onTrigger);
        }
        protected override void Send(UDPSendContext context, Action<UDPDataPack> onTrigger)
        {
            if (!isConnect) return;

            //组成包并取出Buff
            context.Buff = context.Buff ?? new byte[] { };
            var dataPack = new UDPDataPack(context.Type, context.Buff);

            foreach (var packet in dataPack.Packets)
            {
                //发送Buff
                //Tip：发送是不会报错的，除非添加重传机制避免传输失败
                //TODO:所以应该是这样的，包头添加类似ACK机制，并在此刻开始计时，如果在规定时间内收到一个ACK，那么传输未超时
                _client.BeginSend(packet, 0, packet.Length, SocketFlags.None, new AsyncCallback((asyncSend) =>
                {
                    Socket c = (Socket)asyncSend.AsyncState;
                    c.EndSend(asyncSend);

                    MainThreadUtility.Post<UDPDataPack>(onTrigger, dataPack);
                    MainThreadUtility.Post<UDPDataPack>(OnSend, dataPack);
                }), _client);
            }
        }
        protected override void Send(UDPSendContext context, Action<byte[]> onTrigger)
        {
            throw new NotSupportedException();
        }

        //=====断连=====
        /// <summary>
        /// 发起关闭请求(收到关闭包后正式关闭)
        /// </summary>
        public void Disconnect()
        {
            if (!isConnect)
            {
                MLog.Print($"{typeof(MUDPClient)}:客户端已断开连接，请勿重复断连", MLogType.Warning);
                return;
            }

            SendEvent(SocketEvent.C2S_DISCONNECTREQUEST);
        }

        private void DisconnectInternal()
        {
            MLog.Print($"{typeof(MUDPClient)}：客户端已关闭");

            Close();
            MainThreadUtility.Post(OnDisconnect);
        }

        protected override void OnCloseInternal()
        {
            if (!isConnect) return;
            isConnect = false;

            SendEvent(SocketEvent.C2S_DISCONNECTREQUEST);

            _dataBuffer = null;

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

        //=====内部事件=====
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
