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
                            //�رհ�(�ɿͻ�������ر�)
                            if (dataPack.Type == (UInt16)SocketEvent.cs_disconnect)
                            {
                                CloseClient(endPoint);
                                SendBytes(endPoint, SocketEvent.sc_disconnect);
                            }
                            else
                            {
                                Debug.Log($"�յ����Կͻ���<{endPoint}>����Ϣ��{dataPack.ToString()}");
                                MainThreadUtility.Post<EndPoint, SocketDataPack>(OnReceive, endPoint, dataPack);//OnReceive�ص�
                            }
                        }
                    }
                }

                //������������
                ReceiveData();
            }
            catch (Exception e)
            {
                //TODO:Ŀǰֻ����һ�ֿ���Ϊ���������ߣ���֪������û����������
                MLog.Print($"���ݽ���ʧ�ܣ�{e.Message}", MLogType.Warning);
            }
        }

        private void CloseClient(EndPoint ep)
        {
            MainThreadUtility.Post<EndPoint>((socket) =>
            {
                MLog.Print($"���������ڹر�<{ep}>", MLogType.Warning);

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

                    MainThreadUtility.Post<EndPoint, SocketDataPack>(OnSend, endPoint, dataPack);//OnSend�ص�
                }), _server);
            }
            catch (SocketException ex)
            {
                MLog.Print(ex);
            }
        }
    }
}
