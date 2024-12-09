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

        private const int TIMEOUT_CONNECT = 3000;//连接超时时间
        private const int TIMEOUT_SEND = 3000;//发送超时时间
        private const int TIMEOUT_RECEIVE = 3000;//接收超时时间

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

        public void Connect(Action success = null, Action error = null)
        {
            if (isConnecting) return;
            isConnecting = true;

            if (isConnect) MLog.Print($"{typeof(MUDPClient)}：本机已连接至服务器，请勿反复连接", MLogType.Warning);

            Action<bool, string> onTrigger = (flag, ex) =>
            {
                isConnecting = false;

                //成功或失败回调
                if (flag)
                {
                    MLog.Print($"{typeof(MUDPClient)}：客户端已连接至服务器<{_serverEP}>", MLogType.Warning);
                    isConnect = true;

                    MainThreadUtility.Post(success);
                    MainThreadUtility.Post(OnConnectSuccess);
                }
                else
                {
                    MLog.Print($"{typeof(MUDPClient)}：客户端连接至服务器<{_serverEP}>失败：{ex}", MLogType.Warning);

                    MainThreadUtility.Post(error);
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
                _client.Connect(_serverEP);//连接服务器(并非真正连接，为绑定服务器IP)
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
                onTrigger(true, null);

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
                ReConnect(num, index);//失败再次重连
            });
        }

        //发起关闭请求(收到关闭包后正式关闭)
        public void Disconnect()
        {
            SendBytes(SocketEvent.cs_disconnect);
        }
        private void DisconnectInternal()
        {
            MLog.Print("客户端已主动关闭", MLogType.Warning);

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
            //组成包并取出Buff
            context.Buff = context.Buff ?? new byte[] { };
            var dataPack = new SocketDataPack(context.Type, context.Buff);
            var data = dataPack.Buff;

            //发送Buff
            //Tip：发送是不会报错的，除非添加重传机制避免传输失败
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
            //Tip：Socket会自主进行拆包处理(粘包通过包处理)，不需要我们操作
            byte[] bytes = new byte[8 * 1024];//缓冲区大小
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
                    var dataPack = new SocketDataPack();
                    if (_dataBuffer.TryUnpack(out dataPack))
                    {
                        //关闭包
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

                //继续接收数据
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
