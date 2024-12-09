using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
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

        private const int HEAD_CHECKTIME = 5000;//��������ʱ���Ƶ��

        private Timer _headCheckTimer;

        public MUDPServer(string ip, int port) : base(ip, port) { }
        public MUDPServer(IPEndPoint ep) : base(ep) { }

        //=====����=====
        protected override void ReceiveData()
        {
            //Tip��Socket���������в������(ճ��ͨ��������)������Ҫ���ǲ���
            byte[] bytes = new byte[8 * 1024];//��������С
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
                    if (!ClientInfoDic.ContainsKey(endPoint))//���Ӵ���
                    {
                        byte[] verificationBytes = new byte[4] { 18, 203, 59, 38 };
                        byte[] receviedBytes = new byte[4];
                        Array.Copy(bytes, 0, receviedBytes, 0, 4);
                        if (receviedBytes.SequenceEqual(verificationBytes))//��֤ͨ��
                        {
                            //�ͻ������ӻ�Ӧ
                            byte[] buff = new byte[1] { 1 };
                            _server.SendTo(buff, endPoint);
                            //ͨ�������
                            MainThreadUtility.Post<EndPoint>(OnConnect, endPoint);//OnConnect�ص�
                            ClientInfoDic.Add(endPoint, new UDPClientSocketInfo()
                            {
                                Client = endPoint,
                                DataBuffer = new DataBuffer(),
                                HeadTime = MTimeUtility.GetNowTime()
                            });

                            //��������ʱ���
                            _headCheckTimer = new Timer(HEAD_CHECKTIME);
                            _headCheckTimer.AutoReset = true;
                            _headCheckTimer.Elapsed += delegate (object sender, ElapsedEventArgs args)
                            {
                                CheckHeadTimeOut();
                            };
                            _headCheckTimer.Start();

                            MLog.Print($"{typeof(MUDPServer)}���ͻ���<{endPoint}>������");
                        }
                    }
                    else//һ�㴦��
                    {
                        //���ݼ��뻺������(���ݿ��ܷ�������Ҳ����ͬʱ������)
                        ClientInfoDic[endPoint].DataBuffer.AddBuffer(bytes, len);
                        //��ȡ����(�����ȡ)
                        var dataPack = new SocketDataPack();
                        if (ClientInfoDic[endPoint].DataBuffer.TryUnpack(out dataPack))
                        {
                            //������
                            if (dataPack.Type == (UInt16)SocketEvent.C2S_HEAD)
                            {
                                ReceiveHead(endPoint);
                            }
                            //�رհ�(�ɿͻ�������ر�)
                            else if (dataPack.Type == (UInt16)SocketEvent.C2S_DISCONNECT)
                            {
                                ReceiveClose(endPoint);
                            }
                            else
                            {
                                MainThreadUtility.Post<EndPoint, SocketDataPack>(OnReceive, endPoint, dataPack);//OnReceive�ص�
                            }
                        }
                    }
                }

                //������������
                ReceiveData();
            }
            catch (SocketException)
            {
                //TODO:Ŀǰֻ����һ�ֿ���Ϊ���������ߣ���֪������û����������
                MLog.Print($"����������", MLogType.Warning);
            }
        }
        private void ReceiveHead(EndPoint client)
        {
            if (ClientInfoDic.TryGetValue(client, out var info))
            {
                long now = MTimeUtility.GetNowTime();
                long offset = now - info.HeadTime;
                MLog.Print($"�ͻ���<{client}>����������ʱ��� >>>{now}    ��� >>>{offset}");

                if (offset > HEAD_CHECKTIME)
                {
                    //��ʱ(��ʱ�߳��߼�����������ʱ�����ʵ��)
                }
                info.HeadTime = now;//���ģ�����ʱ��
            }
        }
        private void ReceiveClose(EndPoint client) 
        {
            CloseClient(client);
            SendEvent(client, SocketEvent.S2C_DISCONNECT);
        }



        //=====���������=====
        private void CheckHeadTimeOut()
        {
            foreach (var ep in ClientInfoDic.Keys)
            {
                var info = ClientInfoDic[ep];
                long now = MTimeUtility.GetNowTime();
                long offset = now - info.HeadTime;
                if (offset > HEAD_CHECKTIME)
                {
                    //��������ʱ
                    KickOut(ep);
                }
            }
        }



        //=====����=====
        public void SendUTF(EndPoint endPoint, SocketEvent type, string message, Action<EndPoint, SocketDataPack> onTrigger = null)
        {
            byte[] buff = Encoding.UTF8.GetBytes(message);
            UDPSendContext context = new UDPSendContext() { EndPoint = endPoint, Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendASCII(EndPoint endPoint, SocketEvent type, string message, Action<EndPoint, SocketDataPack> onTrigger = null)
        {
            byte[] buff = Encoding.ASCII.GetBytes(message);
            UDPSendContext context = new UDPSendContext() { EndPoint = endPoint, Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendBytes(EndPoint endPoint, SocketEvent type, byte[] buff, Action<EndPoint, SocketDataPack> onTrigger = null)
        {
            UDPSendContext context = new UDPSendContext() { EndPoint = endPoint, Type = (ushort)type, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendEvent(EndPoint endPoint, SocketEvent type, Action<EndPoint, SocketDataPack> onTrigger = null)
        {
            UDPSendContext context = new UDPSendContext() { EndPoint = endPoint, Type = (ushort)type, Buff = null };

            Send(context, onTrigger);
        }
        protected override void Send(UDPSendContext context, Action<EndPoint, SocketDataPack> onTrigger)
        {
            //��ɰ���ȡ��Buff
            context.Buff = context.Buff ?? new byte[] { };
            var dataPack = new SocketDataPack(context.Type, context.Buff);
            var data = dataPack.Buff;

            try
            {
                //����Buff
                _server.BeginSendTo(data, 0, data.Length, SocketFlags.None, context.EndPoint, new AsyncCallback((asyncSend) =>
                {
                    Socket c = (Socket)asyncSend.AsyncState;
                    c.EndSend(asyncSend);

                    MainThreadUtility.Post<EndPoint, SocketDataPack>(onTrigger, endPoint, dataPack);
                    MainThreadUtility.Post<EndPoint, SocketDataPack>(OnSend, endPoint, dataPack);//OnSend�ص�
                }), _server);
            }
            catch (SocketException ex)
            {
                MLog.Print(ex);
            }
        }
        protected override void Send(UDPSendContext context, Action<EndPoint, byte[]> onTrigger = null)
        {
            throw new NotSupportedException();
        }



        //=====����=====
        protected override void OnCloseInternal()
        {
            ClientInfoDic = null;

            OnConnect = null;
            OnDisconnect = null;
            OnReceive = null;
            OnSend = null;

            if (_headCheckTimer != null)
            {
                _headCheckTimer.Stop();
                _headCheckTimer = null;
            }
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
            //Tip��Ϊprivate�����������������Ͽ���ͻ��˵���ϵ�����Ƿ������(��������/�ͻ�������Ҫ��)
            MainThreadUtility.Post<EndPoint>((socket) =>
            {
                MLog.Print($"�������Ͽ���ͻ���<{ep}>������");

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
