using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace MFramework
{
    public class MUDPServer : MUDPServerBase
    {
        public event Action<EndPoint> OnConnect;
        public event Action<EndPoint> OnDisconnect;
        public event Action<EndPoint, SocketDataPack> OnReceive;
        public event Action<EndPoint, SocketDataPack> OnSend;

        public Dictionary<EndPoint, UDPClientSocketInfo> ClientInfoDic =
            new Dictionary<EndPoint, UDPClientSocketInfo>();

        public MUDPServer(string ip, int port) : base(ip, port) { }
        public MUDPServer(IPEndPoint ep) : base(ep) { }

        protected override void ReceiveData()
        {
            //Tip：Socket会自主进行拆包处理(粘包通过包处理)，不需要我们操作
            byte[] bytes = new byte[8 * 1024];//缓冲区大小
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
                        var dataPack = new SocketDataPack();
                        if (ClientInfoDic[endPoint].DataBuffer.TryUnpack(out dataPack))
                        {
                            //关闭包(由客户端请求关闭)
                            if (dataPack.Type == (UInt16)SocketEvent.cs_disconnect)
                            {
                                CloseClient(endPoint);
                                SendBytes(endPoint, SocketEvent.sc_disconnect);
                            }
                            else
                            {
                                Debug.Log($"收到来自客户端<{endPoint}>的消息：{dataPack.ToString()}");
                                MainThreadUtility.Post<EndPoint, SocketDataPack>(OnReceive, endPoint, dataPack);//OnReceive回调
                            }
                        }
                    }
                }

                //继续接收数据
                ReceiveData();
            }
            catch (Exception e)
            {
                //TODO:目前只发现一种可能为服务器断线，不知道还有没有其它可能
                MLog.Print($"数据接收失败：{e.Message}", MLogType.Warning);
            }
        }

        private void CloseClient(EndPoint ep)
        {
            MainThreadUtility.Post<EndPoint>((socket) =>
            {
                MLog.Print($"服务器正在关闭<{ep}>", MLogType.Warning);

                try
                {
                    OnDisconnect?.Invoke(ep);
                    ClientInfoDic.Remove(ep);
                }
                catch { }
            }, ep);
        }


        public void SendUTF(EndPoint endPoint, SocketEvent type, string message = null)
        {
            byte[] buff = Encoding.UTF8.GetBytes(message);
            SendContext context = new SendContext() { EndPoint = endPoint, Type = (ushort)type, Buff = buff };

            Send(context);
        }
        public void SendASCII(EndPoint endPoint, SocketEvent type, string message = null)
        {
            byte[] buff = Encoding.ASCII.GetBytes(message);
            SendContext context = new SendContext() { EndPoint = endPoint, Type = (ushort)type, Buff = buff };

            Send(context);
        }
        public void SendBytes(EndPoint endPoint, SocketEvent type, byte[] buff = null)
        {
            SendContext context = new SendContext() { EndPoint = endPoint, Type = (ushort)type, Buff = buff };

            Send(context);
        }
        protected override void Send(SendContext context)
        {
            //组成包并取出Buff
            context.Buff = context.Buff ?? new byte[] { };
            var dataPack = new SocketDataPack(context.Type, context.Buff);
            var data = dataPack.Buff;

            try
            {
                //发送Buff
                _server.BeginSendTo(data, 0, data.Length, SocketFlags.None, context.EndPoint, new AsyncCallback((asyncSend) =>
                {
                    Socket c = (Socket)asyncSend.AsyncState;
                    c.EndSend(asyncSend);

                    MainThreadUtility.Post<EndPoint, SocketDataPack>(OnSend, endPoint, dataPack);//OnSend回调
                }), _server);
            }
            catch (SocketException ex)
            {
                MLog.Print(ex);
            }
        }
    }
}
