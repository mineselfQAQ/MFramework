using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// Socket客户端
    /// </summary>
    public class MTCPClient
    {
        public string IP;//服务器IP
        public int Port;//服务器Port

        public event Action OnConnectSuccess;
        public event Action OnConnectError;
        public event Action<int> OnReConnectSuccess;
        public event Action<int> OnReConnectError;
        public event Action<int> OnReconnecting;
        public event Action OnDisconnect;
        public event Action<TCPDataPack> OnReceive;
        public event Action<TCPDataPack> OnSend;
        public event Action<SocketException> OnError;

        private const int TIMEOUT_CONNECT = 3000;//连接超时时间
        private const int TIMEOUT_SEND = 3000;//发送超时时间
        private const int TIMEOUT_RECEIVE = 3000;//接收超时时间
        private const int HEAD_OFFSET = 2000;//心跳包发送间隔
        private const int RECONN_MAX_SUM = 3;//最大重连次数

        private Socket _client;
        private Thread _receiveThread;
        private System.Timers.Timer _connTimeoutTimer;
        private System.Timers.Timer _headTimer;
        private DataBuffer _dataBuffer = new DataBuffer();

        public bool isConnect { get; private set; }
        public bool isConnecting { get; private set; }
        public bool isReconnecting { get; private set; }

        /// <summary>
        /// 应用层缓存大小(一次能获取的最大量)
        /// </summary>
        public int BufferSize { get; private set; } = 8198;//默认8KB(+6bytes(包头))
        public int DataSize => BufferSize - 6;

        public MTCPClient(string ip, int port)
        {
            IP = ip;
            Port = port;
        }

        /// <summary>
        /// 设置缓冲区大小(单位KB) 数据大小范围[1024, int.MaxValue - 6]
        /// 注意：如果设置为int.MaxValue，那么实际可存放数据会少6bytes(因为占用了6bytes包头)，如果还是使用int.MaxValue会导致错误
        /// </summary>
        public void SetBufferSize(int size)
        {
            if (size == BufferSize) return;

            size = size * 1024 + 6;
            size = Mathf.Clamp(size, 1030, int.MaxValue);
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        public void Connect(Action onSuccess = null, Action onError = null)
        {
            if (isConnect) MLog.Print($"{typeof(MTCPClient)}：本机已连接至服务器，请勿反复连接", MLogType.Warning);

            if (isConnecting) return;
            isConnecting = true;

            Action<bool> onTrigger = (flag) =>
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
            _connTimeoutTimer = new System.Timers.Timer(TIMEOUT_CONNECT);
            _connTimeoutTimer.AutoReset = false;
            _connTimeoutTimer.Elapsed += delegate (object sender, ElapsedEventArgs args)
            {
                onTrigger(false);
            };
            _connTimeoutTimer.Start();

            try
            {
                _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //Send()/Receive()时等待时间限制
                _client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, TIMEOUT_SEND);
                _client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, TIMEOUT_RECEIVE);
                IPAddress ipAddress = IPAddress.Parse(IP);
                IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, Port);
                //向服务器EP连接
                IAsyncResult result = _client.BeginConnect(ipEndpoint, new AsyncCallback((iar) =>
                {
                    try
                    {
                        Socket client = (Socket)iar.AsyncState;
                        client.EndConnect(iar);//连接成功(如果没超时的话)
                        isConnect = true;

                        //定时发送心跳包
                        _headTimer = new System.Timers.Timer(HEAD_OFFSET);
                        _headTimer.AutoReset = true;
                        _headTimer.Elapsed += delegate (object sender, ElapsedEventArgs args)
                        {
                            SendEvent(SocketEvent.C2S_HEAD);//16位无符号数据(也就是4个16进制数0x1234)
                        };
                        _headTimer.Start();

                        //开启接收数据线程
                        _receiveThread = new Thread(new ThreadStart(ReceiveEvent));
                        _receiveThread.IsBackground = true;//后台线程
                        _receiveThread.Start();

                        onTrigger(true);
                    }
                    catch (SocketException)
                    {
                        onTrigger(false);
                    }
                }), _client);
            }
            catch (SocketException)
            {
                onTrigger(false);
            }
        }
        /// <summary>
        /// 重连服务器
        /// </summary>
        public void ReConnect(int num = RECONN_MAX_SUM, int index = 0)
        {
            isReconnecting = true;

            num--;
            index++;
            if (num < 0)
            {
                OnDisconnectInternal();
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

        public void SendUTF(SocketEvent type, string message, Action<TCPDataPack> onTrigger = null)
        {
            byte[] buff = Encoding.UTF8.GetBytes(message);
            TCPSendContext context = new TCPSendContext() { Socket = _client, Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendASCII(SocketEvent type, string message, Action<TCPDataPack> onTrigger = null)
        {
            byte[] buff = Encoding.ASCII.GetBytes(message);
            TCPSendContext context = new TCPSendContext() { Socket = _client, Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendBytes(SocketEvent type, byte[] buff, Action<TCPDataPack> onTrigger = null)
        {
            TCPSendContext context = new TCPSendContext() { Socket = _client, Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendEvent(SocketEvent type, Action<TCPDataPack> onTrigger = null)
        {
            TCPSendContext context = new TCPSendContext() { Socket = _client, Type = (ushort)type, Buff = null };

            Send(context, onTrigger);
        }
        /// <summary>
        /// 向服务器发送消息
        /// </summary>
        public void Send(TCPSendContext context, Action<TCPDataPack> onTrigger = null)
        {
            //组成包并取出Buff
            context.Buff = context.Buff ?? new byte[] { };
            var dataPack = new TCPDataPack(context.Type, context.Buff);
            var data = dataPack.Buff;

            if (data.Length > BufferSize - 6)
            {
                MLog.Print($"{typeof(MTCPClient)}：传输数据{data.Length}bytes太大，" +
                    $"取值范围[0,{BufferSize - 6}]，如想扩大可调用SetBufferSize()", MLogType.Warning);
                return;
            }

            try
            {
                //发送Buff
                _client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback((asyncSend) =>
                {
                    Socket c = (Socket)asyncSend.AsyncState;
                    c.EndSend(asyncSend);
                    MainThreadUtility.Post<TCPDataPack>(onTrigger, dataPack);
                    MainThreadUtility.Post<TCPDataPack>(OnSend, dataPack);
                }), _client);
            }
            catch (SocketException ex)
            {
                //发不过去则自行断开并重连
                OnErrorInternal(ex);
            }
        }
        /// <summary>
        /// 数据接收线程函数
        /// </summary>
        private void ReceiveEvent()
        {
            while (true)
            {
                try
                {
                    if (!isConnect) break;
                    if (_client.Available <= 0) continue;//如果没有数据，不读取避免堵塞

                    byte[] bytes = new byte[BufferSize];//常规缓冲区大小
                    int len = _client.Receive(bytes);//服务器信息接收(会堵塞)
                    if (len > 0)
                    {
                        //数据加入缓存器中(数据可能分批到达也可能同时到达多个)
                        _dataBuffer.AddBuffer(bytes, len);
                        //获取数据(解包获取)
                        TryUnpack();
                    }
                }
                catch (SocketException ex)
                {
                    //接收出现问题，自行断开并重连
                    OnErrorInternal(ex);
                }
            }
        }

        private void TryUnpack()
        {
            //迭代解包所有包(粘包/网络问题导致的积压)
            while (_dataBuffer.haveBuff) 
            {
                var dataPack = new TCPDataPack();
                if (_dataBuffer.TryUnpack(out dataPack))
                {
                    //踢出包
                    if (dataPack.Type == (UInt16)SocketEvent.S2C_KICKOUT)
                    {
                        OnDisconnectInternal();
                    }
                    //一般情况
                    else
                    {
                        MainThreadUtility.Post<TCPDataPack>(OnReceive, dataPack);
                    }
                }
            }
        }

        /// <summary>
        /// 客户端主动中断连接
        /// </summary>
        public void Disconnect()
        {
            SendEvent(SocketEvent.C2S_DISCONNECTREQUEST);
            OnDisconnectInternal();
        }
        /// <summary>
        /// 客户端完整关闭
        /// </summary>
        public void Close()
        {
            if (!isConnect) return;
            isConnect = false;

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

            if (_receiveThread != null && _receiveThread.IsAlive)
            {
                _receiveThread.Join();
                _receiveThread = null;
            }
            if (_client != null)
            {
                _client.Shutdown(SocketShutdown.Both);
                _client.Close();
                _client = null;
            }
        }

        /// <summary>
        /// 错误回调
        /// </summary>
        /// <param name="e"></param>
        private void OnErrorInternal(SocketException ex)
        {
            Close();

            MainThreadUtility.Post<SocketException>(OnError, ex);

            if (!isReconnecting)
            {
                ReConnect();
            }
        }
        /// <summary>
        /// 断开回调
        /// </summary>
        private void OnDisconnectInternal()
        {
            Close();
            MainThreadUtility.Post(OnDisconnect);
        }
    }
}