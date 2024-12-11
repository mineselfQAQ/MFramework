using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// Socket服务端
    /// </summary>
    public class MTCPServer
    {
        public string IP;//服务器IP
        public int Port;//服务器Port

        public event Action<Socket> OnConnect;
        public event Action<Socket> OnDisconnect;
        public event Action<Socket, TCPDataPack> OnReceive;
        public event Action<Socket, TCPDataPack> OnSend;

        public Dictionary<Socket, TCPClientSocketInfo> ClientInfoDic = new Dictionary<Socket, TCPClientSocketInfo>();

        private const int HEAD_CHECKTIME = 5000;//心跳包定时检测频率

        private Socket _server;
        private Thread _connectThread;
        private System.Timers.Timer _headCheckTimer;
        private DataBuffer _dataBuffer = new DataBuffer();

        public bool isValid { get; private set; }
        /// <summary>
        /// 应用层缓存大小(一次能获取的最大量)
        /// </summary>
        public int BufferSize { get; private set; } = 8198;//默认8KB(+6bytes(包头))
        public int DataSize => BufferSize - 6;

        /// <summary>
        /// 设置缓冲区大小(单位KB) 范围[1024, int.MaxValue]
        /// 注意：如果设置为int.MaxValue，那么实际容量会少几bytes，如果还是使用int.MaxValue会导致错误
        /// </summary>
        public void SetBufferSize(int size)
        {
            if (size == BufferSize) return;

            size = size * 1024 + 128;//冗余128bytes
            size = Mathf.Clamp(size, 1152, int.MaxValue);
        }

        public MTCPServer(string ip, int port)
        {
            IP = ip;
            Port = port;

            isValid = true;

            IPAddress ipAddress = IPAddress.Parse(IP);
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _server.Bind(new IPEndPoint(ipAddress, Port));//绑定自己

            _server.Listen(10);//设定最多10个排队连接请求(并非总连接数)

            //启动线程监听连接
            _connectThread = new Thread(ListenClientConnect);
            _connectThread.Start();

            //心跳包定时检测
            _headCheckTimer = new System.Timers.Timer(HEAD_CHECKTIME);
            _headCheckTimer.AutoReset = true;
            _headCheckTimer.Elapsed += delegate (object sender, ElapsedEventArgs args)
            {
                CheckHeadTimeOut();
            };
            _headCheckTimer.Start();
        }

        public void SendUTF(Socket socket, SocketEvent type, string message, Action<Socket, TCPDataPack> onTrigger = null)
        {
            byte[] buff = Encoding.UTF8.GetBytes(message);
            TCPSendContext context = new TCPSendContext() { Socket = socket, Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendASCII(Socket socket, SocketEvent type, string message, Action<Socket, TCPDataPack> onTrigger = null)
        {
            byte[] buff = Encoding.ASCII.GetBytes(message);
            TCPSendContext context = new TCPSendContext() { Socket = socket, Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendBytes(Socket socket, SocketEvent type, byte[] buff, Action<Socket, TCPDataPack> onTrigger = null)
        {
            TCPSendContext context = new TCPSendContext() { Socket = socket, Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendEvent(Socket socket, SocketEvent type, Action<Socket, TCPDataPack> onTrigger = null)
        {
            TCPSendContext context = new TCPSendContext() { Socket = socket, Type = (ushort)type, Buff = null };

            Send(context, onTrigger);
        }

        /// <summary>
        /// 向客户端发送消息
        /// </summary>
        public void Send(TCPSendContext context, Action<Socket, TCPDataPack> onTrigger)
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
                context.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback((asyncSend) =>
                {
                    try
                    {
                        Socket c = (Socket)asyncSend.AsyncState;
                        c.EndSend(asyncSend);
                        MainThreadUtility.Post<Socket, TCPDataPack>(onTrigger, context.Socket, dataPack);
                        MainThreadUtility.Post<Socket, TCPDataPack>(OnSend, context.Socket, dataPack);
                    }
                    catch (SocketException)
                    {
                        //发不过去则关闭该客户端的连接
                        CloseClient(context.Socket);
                    }
                }), context.Socket);
            }
            catch (SocketException)
            {
                //发不过去则关闭该客户端的连接
                CloseClient(context.Socket);
            }
        }

        /// <summary>  
        /// 监听客户端连接  
        /// </summary>  
        private void ListenClientConnect()
        {
            while (true)
            {
                try
                {
                    if (!isValid) break;
                    Socket client = _server.Accept();//客户端连接(会堵塞)

                    Thread receiveThread = new Thread(ReceiveEvent);//子线程
                    ClientInfoDic.Add(client, new TCPClientSocketInfo() 
                        { Client = client, 
                          ReceiveThread = receiveThread,
                          HeadTime = MTimeUtility.GetNowTime() });
                    receiveThread.Start(client);
                    //Tip：和协程一样，子线程开启会继续执行后续代码
                    //所以Accept()成功后，即连接成功
                    MainThreadUtility.Post<Socket>(OnConnect, client);
                }
                catch
                {
                    break;
                }
            }
        }
        /// <summary>
        /// 数据接收线程函数
        /// </summary>
        private void ReceiveEvent(object client)
        {
            Socket socket = (Socket)client;
            while (true)
            {
                if (!isValid) return;
                if (!ClientInfoDic.ContainsKey(socket))
                {
                    return;
                }

                try
                {
                    byte[] bytes = new byte[BufferSize];//常规缓冲区大小
                    int len = socket.Receive(bytes);//客户端信息接收(会堵塞)
                    if (len > 0)
                    {
                        //数据加入缓存器中(数据可能分批到达也可能同时到达多个)
                        _dataBuffer.AddBuffer(bytes, len);
                        //获取数据(解包获取)
                        TryUnpack(socket);
                    }
                    else
                    {
                        //有数据能读，但是Receive()得到0，即客户端Socket被关闭
                        if (socket.Poll(-1, SelectMode.SelectRead))
                        {
                            //**在退出程序时同样会触发**
                            CloseClient(socket);
                            return;
                        }
                    }
                }
                catch (SocketException)
                {
                    //接收出现问题，关闭该客户端的连接
                    CloseClient(socket);
                    return;
                }
            }
        }

        private void TryUnpack(Socket socket)
        {
            //迭代解包所有包(粘包/网络问题导致的积压)
            while (_dataBuffer.haveBuff)
            {
                var dataPack = new TCPDataPack();
                if (_dataBuffer.TryUnpack(out dataPack))
                {
                    //心跳包
                    if (dataPack.Type == (UInt16)SocketEvent.C2S_HEAD)
                    {
                        ReceiveHead(socket);
                    }
                    //断开连接
                    else if (dataPack.Type == (UInt16)SocketEvent.C2S_DISCONNECTREQUEST)
                    {
                        CloseClient(socket);
                    }
                    //一般情况
                    else
                    {
                        MainThreadUtility.Post<Socket, TCPDataPack>(OnReceive, socket, dataPack);
                    }
                }
            }
        }

        /// <summary>
        /// 接收到心跳包
        /// </summary>
        private void ReceiveHead(Socket client)
        {
            if (ClientInfoDic.TryGetValue(client, out var info))
            {
                long now = MTimeUtility.GetNowTime();
                long offset = now - info.HeadTime;
                MLog.Print($"客户端<{client.LocalEndPoint}>：更新心跳时间戳 >>>{now}    间隔 >>>{offset}");

                if (offset > HEAD_CHECKTIME)
                {
                    //超时(超时踢出逻辑在心跳包定时检测中实现)
                }
                info.HeadTime = now;//核心：更新时间
            }
        }
        /// <summary>
        /// 检测心跳包超时
        /// </summary>
        private void CheckHeadTimeOut()
        {
            foreach (var socket in ClientInfoDic.Keys)
            {
                var info = ClientInfoDic[socket];
                long now = MTimeUtility.GetNowTime();
                long offset = now - info.HeadTime;
                if (offset > HEAD_CHECKTIME)
                {
                    //心跳包超时
                    KickOut(socket);
                }
            }
        }

        /// <summary>
        /// 将客户端踢出
        /// </summary>
        public void KickOut(Socket client)
        {
            SendEvent(client, SocketEvent.S2C_KICKOUT, (socket, dataPack) =>
            {
                CloseClient(client);
            });
        }
        /// <summary>
        /// 踢出所有客户端
        /// </summary>
        public void KickOutAll()
        {
            foreach (var socket in ClientInfoDic.Keys)
            {
                KickOut(socket);
            }
        }

        /// <summary>
        /// 服务器完整关闭
        /// </summary>
        public void Close()
        {
            if (!isValid) return;
            isValid = false;

            foreach (var socket in ClientInfoDic.Keys)
            {
                CloseClient(socket);
            }

            if (_headCheckTimer != null)
            {
                _headCheckTimer.Stop();
                _headCheckTimer = null;
            }

            ClientInfoDic = null;
            _dataBuffer = null;

            OnConnect = null;
            OnDisconnect = null;
            OnReceive = null;
            OnSend = null;

            _server.Close();
            _server = null;
        }

        private void CloseClient(Socket client)
        {
            MainThreadUtility.Post<Socket>((socket) =>
            {
                try
                {
                    OnDisconnect?.Invoke(socket);
                    ClientInfoDic.Remove(socket);
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch { }
            }, client);
        }
    }
}