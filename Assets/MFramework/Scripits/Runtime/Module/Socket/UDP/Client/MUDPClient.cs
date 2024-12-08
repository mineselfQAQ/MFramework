using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine;

namespace MFramework
{
    public class MUDPClient
    {
        public bool isConnect { get; private set; }

        private Socket _client;
        private EndPoint _serverEP;//服务器地址
        private DataBuffer _dataBuffer = new DataBuffer();

        public MUDPClient(string ip, int port)
        {
            //服务器参数设置
            var ep = new IPEndPoint(IPAddress.Parse(ip), port);

            InitSettings(ep);
        }
        public MUDPClient(IPEndPoint ep)
        {
            InitSettings(ep);
        }

        public void InitSettings(IPEndPoint ep)
        {
            _serverEP = ep;

            _client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            isConnect = false;
        }

        public void Connect()
        {
            if (isConnect) MLog.Print($"{typeof(MUDPClient)}：本机已连接至服务器，请勿反复连接", MLogType.Warning);

            try
            {
                _client.Bind(new IPEndPoint(IPAddress.Any, 0));//绑定自己
                _client.Connect(_serverEP);//连接服务器
            }
            catch (SocketException e)
            {
                throw new Exception($"绑定失败：{e}");
            }
            //向服务器发送验证
            byte[] buff = new byte[4] { 18, 203, 59, 38 };//任意四个数
            _client.Send(buff);
            buff = new byte[1];
            _client.ReceiveTimeout = 5000;//开启接收
            int len = _client.Receive(buff);
            _client.ReceiveTimeout = 0;//关闭接收
            if (len != 1 || buff[0] != 1)
            {
                MLog.Print("连接验证失败", MLogType.Error);
            }
            MLog.Print($"{typeof(MUDPClient)}：客户端已连接至服务器<{_serverEP}>");
            isConnect = true;

            ReceiveData();//开启数据接收
        }

        public void Send(UInt16 e, byte[] buff = null)
        {
            //组成包并取出Buff
            buff = buff ?? new byte[] { };
            var dataPack = new SocketDataPack(e, buff);
            var data = dataPack.Buff;

            try
            {
                //发送Buff
                _client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback((asyncSend) =>
                {
                    Socket c = (Socket)asyncSend.AsyncState;
                    c.EndSend(asyncSend);
                }), _client);
            }
            catch (SocketException ex)
            {
                MLog.Print(ex);
            }
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
                        //if (dataPack.Type == (UInt16)SocketEvent.xxx)
                        //{

                        //}
                        
                        Debug.Log($"收到来自服务器<{_serverEP}>的消息：{dataPack.ToString()}");
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
    }
}
