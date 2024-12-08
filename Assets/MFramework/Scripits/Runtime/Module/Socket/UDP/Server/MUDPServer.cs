using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace MFramework
{
    public class MUDPServer
    {
        public string IP;//服务器IP
        public int Port;//服务器Port
        public EndPoint EP;//服务器EP

        public event Action<EndPoint> OnConnect;
        public event Action<EndPoint> OnDisconnect;//TODO:待完成
        public event Action<EndPoint, SocketDataPack> OnReceive;
        public event Action<EndPoint, SocketDataPack> OnSend;

        public Dictionary<EndPoint, UDPClientSocketInfo> ClientInfoDic;

        private Socket _server;
        //private List<EndPoint> _clients;//连接上的客户端
        //private Dictionary<EndPoint, DataBuffer> _bufferDic;//客户端缓存字典
        private EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);//任意客户端EndPoint

        public MUDPServer(string ip, int port)
        {
            IP = ip;
            Port = port;
            EP = new IPEndPoint(IPAddress.Parse(ip), port);

            InitSettings((IPEndPoint)EP);
        }
        public MUDPServer(IPEndPoint ep)
        {
            EP = ep;
            IP = ep.Address.ToString();
            Port = ep.Port;

            InitSettings(ep);
        }

        public void InitSettings(IPEndPoint ep)
        {
            ClientInfoDic = new Dictionary<EndPoint, UDPClientSocketInfo>();
            //_clients = new List<EndPoint>();
            //_bufferDic = new Dictionary<EndPoint, DataBuffer>();

            _server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _server.Bind(ep);

            MLog.Print($"{typeof(MUDPServer)}：服务器<{_server.LocalEndPoint}>已开始监听");

            ReceiveData();
        }

        public void Quit()
        {
            _server.Close();
        }

        private void ReceiveData()
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
                            OnConnect?.Invoke(endPoint);
                            ClientInfoDic.Add(endPoint, new UDPClientSocketInfo() 
                                { Client = endPoint,
                                  DataBuffer = new DataBuffer(), 
                                  HeadTime = MTimeUtility.GetNowTime() });
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
                            Debug.Log($"收到来自客户端<{endPoint}>的消息：{dataPack.ToString()}");
                            Send(endPoint, (UInt16)SocketEvent.test, MConvertUtility.UTF8ToBytes("已收到"));
                        }
                    }
                }

                //继续接收数据
                ReceiveData();
            }
            catch (Exception e)
            {
                MLog.Print("数据接收失败：" + e.Message, MLogType.Warning);
            }
        }

        public void Send(EndPoint endPoint, UInt16 e, byte[] buff = null)
        {
            //组成包并取出Buff
            buff = buff ?? new byte[] { };
            var dataPack = new SocketDataPack(e, buff);
            var data = dataPack.Buff;

            try
            {
                //发送Buff
                _server.BeginSendTo(data, 0, data.Length, SocketFlags.None, endPoint, new AsyncCallback((asyncSend) =>
                {
                    Socket c = (Socket)asyncSend.AsyncState;
                    c.EndSend(asyncSend);
                }), _server);
            }
            catch (SocketException ex)
            {
                MLog.Print(ex);
            }
        }
    }
}
