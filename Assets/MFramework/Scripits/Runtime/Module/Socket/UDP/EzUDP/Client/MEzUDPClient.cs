using System.Net.Sockets;
using System.Net;
using System;
using System.Text;

namespace MFramework
{
    /// <summary>
    /// 简易版UDP客户端，不提供任何多余功能，仅支持直接传输
    /// </summary>
    public class MEzUDPClient : MUDPClientBase
    {
        public MEzUDPClient(string ip, int port) : base(ip, port) { }
        public MEzUDPClient(IPEndPoint ep) : base(ep) { }

        public event Action<EndPoint, byte[]> OnSend;

        public void SendUTF(string message = null, Action<SocketDataPack> onTrigger = null)
        {
            byte[] buff = Encoding.UTF8.GetBytes(message);
            SendContext context = new SendContext() { EndPoint = _serverEP, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendASCII(string message = null, Action<SocketDataPack> onTrigger = null)
        {
            byte[] buff = Encoding.ASCII.GetBytes(message);
            SendContext context = new SendContext() { EndPoint = _serverEP, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendBytes(byte[] buff = null, Action<SocketDataPack> onTrigger = null)
        {
            SendContext context = new SendContext() { EndPoint = _serverEP, Buff = buff };

            Send(context, onTrigger);
        }
        protected override void Send(SendContext context, Action<SocketDataPack> onTrigger)
        {
            try
            {
                //发送Buff
                _client.BeginSendTo(context.Buff, 0, context.Buff.Length, SocketFlags.None, context.EndPoint, new AsyncCallback((asyncSend) =>
                {
                    Socket c = (Socket)asyncSend.AsyncState;
                    c.EndSend(asyncSend);

                    MainThreadUtility.Post<EndPoint, byte[]>(OnSend, context.EndPoint, context.Buff);//OnSend回调
                }), _client);
            }
            catch (SocketException ex)
            {
                MLog.Print(ex);
            }
        }
    }
}
