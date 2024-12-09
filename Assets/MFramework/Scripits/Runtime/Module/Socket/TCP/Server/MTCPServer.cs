using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;

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
        public event Action<Socket, SocketDataPack> OnReceive;
        public event Action<Socket, SocketDataPack> OnSend;

        public Dictionary<Socket, TCPClientSocketInfo> ClientInfoDic = new Dictionary<Socket, TCPClientSocketInfo>();

        private const int HEAD_TIMEOUT = 5000;//客户端心跳超时时间
        private const int HEAD_CHECKTIME = 5000;//心跳包定时检测频率

        private Socket _server;
        private Thread _connectThread;
        private System.Timers.Timer _headCheckTimer;
        private DataBuffer _dataBuffer = new DataBuffer();

        private bool _isValid = true;

        public MTCPServer(string ip, int port)
        {
            IP = ip;
            Port = port;

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

        /// <summary>
        /// 向客户端发送消息
        /// </summary>
        public void Send(Socket client, UInt16 e, byte[] buff = null, Action<SocketDataPack> onTrigger = null)
        {
            //组成包并取出Buff
            buff = buff ?? new byte[] { };
            var dataPack = new SocketDataPack(e, buff);
            var data = dataPack.Buff;

            try
            {
                //发送Buff
                client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback((asyncSend) =>
                {
                    try
                    {
                        Socket c = (Socket)asyncSend.AsyncState;
                        c.EndSend(asyncSend);
                        MainThreadUtility.Post<SocketDataPack>(onTrigger, dataPack);
                        MainThreadUtility.Post<Socket, SocketDataPack>(OnSend, client, dataPack);
                    }
                    catch (SocketException)
                    {
                        //发不过去则关闭该客户端的连接
                        CloseClient(client);
                    }
                }), client);
            }
            catch (SocketException)
            {
                //发不过去则关闭该客户端的连接
                CloseClient(client);
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
                    if (!_isValid) break;
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
                if (!_isValid) return;
                if (!ClientInfoDic.ContainsKey(socket))
                {
                    return;
                }

                try
                {
                    byte[] bytes = new byte[8 * 1024];//常规缓冲区大小
                    int len = socket.Receive(bytes);//客户端信息接收(会堵塞)
                    if (len > 0)
                    {
                        //数据加入缓存器中(数据可能分批到达也可能同时到达多个)
                        //假如是大数据，那么会每次接收8192bytes的形式进行，最终在_dataBuffer的数据是一致的
                        //假如是粘包数据，那么
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
            var dataPack = new SocketDataPack();
            if (_dataBuffer.TryUnpack(out dataPack))
            {
                //心跳包
                if (dataPack.Type == (UInt16)SocketEvent.cs_head)
                {
                    ReceiveHead(socket);
                }
                //断开连接
                else if (dataPack.Type == (UInt16)SocketEvent.cs_disconnect)
                {
                    CloseClient(socket);
                }
                //一般情况
                else
                {
                    MainThreadUtility.Post<Socket, SocketDataPack>(OnReceive, socket, dataPack);
                }

                if(_dataBuffer.haveBuff) TryUnpack(socket);
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
                MLog.Print($"更新心跳时间戳 >>>{now}    间隔 >>>{offset}");

                if (offset > HEAD_TIMEOUT)
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
                if (offset > HEAD_TIMEOUT)
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
            Send(client, (UInt16)SocketEvent.sc_kickout, null, (dataPack) =>
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
            if (!_isValid) return;
            _isValid = false;

            foreach (var socket in ClientInfoDic.Keys)
            {
                CloseClient(socket);
            }

            if (_headCheckTimer != null)
            {
                _headCheckTimer.Stop();
                _headCheckTimer = null;
            }

            _server.Close();
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