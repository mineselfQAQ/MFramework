using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MFramework
{
    /// <summary>
    /// 简易版UDP服务器，不提供任何多余功能，仅支持直接传输
    /// </summary>
    public class MEzUDPServer : MUDPServerBase
    {
        public MEzUDPServer(string ip, int port) : base(ip, port) { }
        public MEzUDPServer(IPEndPoint ep) : base(ep) { }

        public event Action<EndPoint, string> OnReceive;
        public event Action<EndPoint, byte[]> OnSend;

        protected override void ReceiveData()
        {
            //Tip：数据量不得超过64KB，否则会截断从而无法获取
            byte[] bytes = new byte[64 * 1024];//最大缓冲区大小
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
                    string message = Encoding.UTF8.GetString(bytes);

                    MLog.Print($"收到来自客户端<{endPoint}>的消息：{message}");
                    MainThreadUtility.Post<EndPoint, string>(OnReceive, endPoint, message);//OnReceive回调
                }

                //继续接收数据
                ReceiveData();
            }
            catch (Exception e)
            {
                MLog.Print("数据接收失败：" + e.Message, MLogType.Warning);
            }
        }

        public void SendUTF(EndPoint endPoint, string message = null)
        {
            byte[] buff = Encoding.UTF8.GetBytes(message);
            SendContext context = new SendContext() { EndPoint = endPoint, Buff = buff };

            Send(context);
        }
        public void SendASCII(EndPoint endPoint, string message = null)
        {
            byte[] buff = Encoding.ASCII.GetBytes(message);
            SendContext context = new SendContext() { EndPoint = endPoint, Buff = buff };

            Send(context);
        }
        public void SendBytes(EndPoint endPoint, byte[] buff = null)
        {
            SendContext context = new SendContext() { EndPoint = endPoint, Buff = buff };

            Send(context);
        }
        protected override void Send(SendContext context)
        {
            try
            {
                //发送Buff
                _server.BeginSendTo(context.Buff, 0, context.Buff.Length, SocketFlags.None, context.EndPoint, new AsyncCallback((asyncSend) =>
                {
                    Socket c = (Socket)asyncSend.AsyncState;
                    c.EndSend(asyncSend);

                    MainThreadUtility.Post<EndPoint, byte[]>(OnSend, context.EndPoint, context.Buff);//OnSend回调
                }), _server);
            }
            catch (SocketException ex)
            {
                MLog.Print(ex);
            }
        }
    }
}
