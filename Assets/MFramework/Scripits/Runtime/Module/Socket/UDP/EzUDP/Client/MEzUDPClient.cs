using System.Net.Sockets;
using System.Net;
using System;
using System.Text;

namespace MFramework
{
    /// <summary>
    /// ���װ�UDP�ͻ��ˣ����ṩ�κζ��๦�ܣ���֧��ֱ�Ӵ���
    /// </summary>
    public class MEzUDPClient : MUDPClientBase
    {
        public MEzUDPClient(string ip, int port) : base(ip, port) { }
        public MEzUDPClient(IPEndPoint ep) : base(ep) { }

        public event Action<byte[]> OnSend;

        protected override void OnCloseInternal()
        {
            OnSend = null;
        }

        public void SendUTF(string message, Action<byte[]> onTrigger = null)
        {
            byte[] buff = Encoding.UTF8.GetBytes(message);
            UDPSendContext context = new UDPSendContext() { EndPoint = serverEP, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendASCII(string message, Action<byte[]> onTrigger = null)
        {
            byte[] buff = Encoding.ASCII.GetBytes(message);
            UDPSendContext context = new UDPSendContext() { EndPoint = serverEP, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendBytes(byte[] buff, Action<byte[]> onTrigger = null)
        {
            UDPSendContext context = new UDPSendContext() { EndPoint = serverEP, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendEvent(Action<byte[]> onTrigger = null)
        {
            UDPSendContext context = new UDPSendContext() { EndPoint = serverEP, Buff = null };

            Send(context, onTrigger);
        }
        protected override void Send(UDPSendContext context, Action<byte[]> onTrigger)
        {
            try
            {
                //����Buff
                _client.BeginSendTo(context.Buff, 0, context.Buff.Length, SocketFlags.None, context.EndPoint, new AsyncCallback((asyncSend) =>
                {
                    Socket c = (Socket)asyncSend.AsyncState;
                    c.EndSend(asyncSend);

                    MainThreadUtility.Post<byte[]>(onTrigger, context.Buff);//OnSend�ص�
                    MainThreadUtility.Post<byte[]>(OnSend, context.Buff);//OnSend�ص�
                }), _client);
            }
            catch (SocketException ex)
            {
                MLog.Print(ex);
            }
        }
        protected override void Send(UDPSendContext context, Action<SocketDataPack> onTrigger)
        {
            throw new NotSupportedException();
        }
    }
}