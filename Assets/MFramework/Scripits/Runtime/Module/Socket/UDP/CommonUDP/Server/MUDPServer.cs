using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;

namespace MFramework
{
    public class MUDPServer : MUDPServerBase
    {
        public event Action<EndPoint> OnConnect;
        public event Action<EndPoint> OnDisconnect;
        public event Action<EndPoint, UDPDataPack> OnReceive;
        public event Action<EndPoint, UDPDataPack> OnSend;

        public Dictionary<EndPoint, UDPClientSocketInfo> ClientInfoDic =
            new Dictionary<EndPoint, UDPClientSocketInfo>();

        private const int HEAD_CHECKTIME = 5000;//心跳包定时检测频率

        private System.Timers.Timer _headCheckTimer;

        public MUDPServer(string ip, int port) : base(ip, port)
        {
            StartHeadCheckTimer();
        }
        public MUDPServer(IPEndPoint ep) : base(ep)
        {
            StartHeadCheckTimer();
        }

        //===开启===
        public override void Open()
        {
            base.Open();
            if (_headCheckTimer == null)
            {
                StartHeadCheckTimer();
            }
        }
        private void StartHeadCheckTimer()
        {
            //心跳包定时检测
            _headCheckTimer = new System.Timers.Timer(HEAD_CHECKTIME);
            _headCheckTimer.AutoReset = true;
            _headCheckTimer.Elapsed += delegate (object sender, ElapsedEventArgs args)
            {
                CheckHeadTimeOut();
            };
            _headCheckTimer.Start();
        }

        //=====接收=====
        protected override void ReceiveData()
        {
            //1024---小于链路层负载(1472)的值
            byte[] bytes = new byte[1024];//缓冲区大小
            _server.BeginReceiveFrom(bytes, 0, bytes.Length, SocketFlags.None, ref endPoint, new AsyncCallback(OnReceiveData), bytes);
        }
        private void OnReceiveData(IAsyncResult result)
        {
            try
            {
                byte[] bytes = (byte[])result.AsyncState;
                int len = _server.EndReceiveFrom(result, ref endPoint);
                if (len > 0)
                {
                    if (!ClientInfoDic.ContainsKey(endPoint))//连接处理
                    {
                        byte[] verificationBytes = new byte[4] { 18, 203, 59, 38 };
                        byte[] receviedBytes = new byte[4];
                        Array.Copy(bytes, 0, receviedBytes, 0, 4);
                        if (receviedBytes.SequenceEqual(verificationBytes))//验证通过
                        {
                            //客户端连接回应
                            byte[] buff = new byte[1] { 1 };
                            _server.SendTo(buff, endPoint);
                            //通过后操作
                            MainThreadUtility.Post<EndPoint>(OnConnect, endPoint);//OnConnect回调
                            ClientInfoDic.Add(endPoint, new UDPClientSocketInfo()
                            {
                                Client = endPoint,
                                DataBuffer = new DataBuffer(),
                                HeadTime = MTimeUtility.GetNowTime()
                            });

                            MLog.Print($"{typeof(MUDPServer)}：客户端<{endPoint}>已连接");
                        }
                    }
                    else//一般处理
                    {
                        //数据加入缓存器中(数据可能分批到达也可能同时到达多个)
                        ClientInfoDic[endPoint].DataBuffer.AddBuffer(bytes, len);
                        //获取数据(解包获取)
                        TryUnpack(endPoint);
                    }
                }

                //继续接收数据
                ReceiveData();
            }
            catch (SocketException)
            {

            }
        }
        private void ReceiveHead(EndPoint client)
        {
            if (ClientInfoDic.TryGetValue(client, out var info))
            {
                long now = MTimeUtility.GetNowTime();
                long offset = now - info.HeadTime;
                MLog.Print($"客户端<{client}>：更新心跳时间戳 >>>{now}    间隔 >>>{offset}");

                if (offset > HEAD_CHECKTIME)
                {
                    //超时(超时踢出逻辑在心跳包定时检测中实现)
                }
                info.HeadTime = now;//核心：更新时间
            }
        }
        private void ReceiveCloseRequest(EndPoint client) 
        {
            CloseClient(client);
            SendEvent(client, SocketEvent.S2C_DISCONNECTREPLY);
        }
        private void ReceiveCloseReply(EndPoint client)
        {
            CloseClient(client);
        }

        private void TryUnpack(EndPoint ep)
        {
            //迭代解包所有包(网络问题导致的积压)
            if (ClientInfoDic[endPoint].DataBuffer.haveBuff)
            {
                var dataPack = new UDPDataPack();
                if (ClientInfoDic[endPoint].DataBuffer.TryUnpack(out dataPack))
                {
                    //心跳包
                    if (dataPack.Type == (UInt16)SocketEvent.C2S_HEAD)
                    {
                        ReceiveHead(endPoint);
                    }
                    //关闭包(客户端请求关闭)
                    else if (dataPack.Type == (UInt16)SocketEvent.C2S_DISCONNECTREQUEST)
                    {
                        ReceiveCloseRequest(endPoint);
                    }
                    //关闭包(客户端关闭回复)
                    else if (dataPack.Type == (UInt16)SocketEvent.C2S_DISCONNECTREPLY)
                    {
                        ReceiveCloseReply(endPoint);
                    }
                    else
                    {
                        MainThreadUtility.Post<EndPoint, UDPDataPack>(OnReceive, ep, dataPack);//OnReceive回调
                    }
                }
            }
        }

        //=====心跳包检测=====
        private void CheckHeadTimeOut()
        {
            foreach (var ep in ClientInfoDic.Keys)
            {
                var info = ClientInfoDic[ep];
                long now = MTimeUtility.GetNowTime();
                long offset = now - info.HeadTime;
                if (offset > HEAD_CHECKTIME)
                {
                    //心跳包超时
                    KickOut(ep);
                }
            }
        }



        //=====发送=====
        public void SendUTF(EndPoint endPoint, SocketEvent type, string message, Action<EndPoint, UDPDataPack> onTrigger = null)
        {
            byte[] buff = Encoding.UTF8.GetBytes(message);
            UDPSendContext context = new UDPSendContext() { EndPoint = endPoint, Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendASCII(EndPoint endPoint, SocketEvent type, string message, Action<EndPoint, UDPDataPack> onTrigger = null)
        {
            byte[] buff = Encoding.ASCII.GetBytes(message);
            UDPSendContext context = new UDPSendContext() { EndPoint = endPoint, Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendBytes(EndPoint endPoint, SocketEvent type, byte[] buff, Action<EndPoint, UDPDataPack> onTrigger = null)
        {
            UDPSendContext context = new UDPSendContext() { EndPoint = endPoint, Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendEvent(EndPoint endPoint, SocketEvent type, Action<EndPoint, UDPDataPack> onTrigger = null)
        {
            UDPSendContext context = new UDPSendContext() { EndPoint = endPoint, Type = (ushort)type, Buff = null };

            Send(context, onTrigger);
        }
        protected override void Send(UDPSendContext context, Action<EndPoint, UDPDataPack> onTrigger)
        {
            //组成包并取出Buff
            context.Buff = context.Buff ?? new byte[] { };
            var dataPack = new UDPDataPack(context.Type, context.Buff);

            foreach (var packet in dataPack.Packets)
            {
                //发送Buff
                _server.BeginSendTo(packet, 0, packet.Length, SocketFlags.None, context.EndPoint, new AsyncCallback((asyncSend) =>
                {
                    Socket c = (Socket)asyncSend.AsyncState;
                    c.EndSend(asyncSend);

                    MainThreadUtility.Post<EndPoint, UDPDataPack>(onTrigger, endPoint, dataPack);
                    MainThreadUtility.Post<EndPoint, UDPDataPack>(OnSend, endPoint, dataPack);//OnSend回调
                }), _server);
            }
        }
        protected override void Send(UDPSendContext context, Action<EndPoint, byte[]> onTrigger = null)
        {
            throw new NotSupportedException();
        }



        //=====断连=====
        protected override void OnCloseInternal()
        {
            if (ClientInfoDic.Keys.Count == 0) return;

            //先对所有客户端进行断连操作(发送断连报文)
            foreach (var ep in ClientInfoDic.Keys)
            {
                SendEvent(ep, SocketEvent.S2C_DISCONNECTREQUEST);
            }

            isWaiting = true;//协程等待

            //条件：等待5秒 或 收到所有回报
            //Tip：只能使用线程，在OnApplicationQuit()时协程已失效
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Check();
            });
        }

        private void Check()
        {
            int elapsed = 0;
            while (elapsed < 5000)//等待5秒
            {
                if (ClientInfoDic.Keys.Count == 0)
                {
                    break;
                }

                Thread.Sleep(100);//每100ms检查一次
                elapsed += 100;
            }

            CloseInternal();
        }
        private void CloseInternal()
        {
            ClientInfoDic = null;

            if (_headCheckTimer != null)
            {
                _headCheckTimer.Stop();
                _headCheckTimer = null;
            }

            isWaiting = false;//继续执行
        }

        public void KickOutAll()
        {
            foreach (var ep in ClientInfoDic.Keys)
            {
                KickOut(ep);
            }
        }
        public void KickOut(EndPoint client)
        {
            SendEvent(client, SocketEvent.S2C_KICKOUT, (ep, dataPack) =>
            {
                CloseClient(client);
            });
        }

        private void CloseClient(EndPoint ep)
        {
            //Tip：为private，服务器不会主动断开与客户端的联系，除非发生情况(如心跳包/客户端自主要求)
            MainThreadUtility.Post<EndPoint>((socket) =>
            {
                MLog.Print($"服务器断开与客户端<{ep}>的连接");

                try
                {
                    OnDisconnect?.Invoke(ep);
                    ClientInfoDic.Remove(ep);
                }
                catch { }
            }, ep);
        }
    }
}
