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
        public MEzUDPServer(string ip, int port) : base(ip, port)
        {
            //一次性传输，设置为最大
            _server.SendBufferSize = 65507;
            _server.ReceiveBufferSize = 65507;
        }
        public MEzUDPServer(IPEndPoint ep) : base(ep)
        {
            //一次性传输，设置为最大
            _server.SendBufferSize = 65507;
            _server.ReceiveBufferSize = 65507;
        }

        public event Action<EndPoint, string> OnReceive;
        public event Action<EndPoint, byte[]> OnSend;

        protected override void ReceiveData()
        {
            //UDP只要发送超出限制则失败，所以设置为最大值
            byte[] bytes = new byte[64 * 1024 - 20 - 8];//最大缓冲区大小
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
            }
            catch (SocketException ex)
            {
                MLog.Print("数据接收失败：" + ex.Message, MLogType.Warning);
            }
            finally
            {
                //继续接收数据
                ReceiveData();
            }
        }

        public void SendUTF(EndPoint endPoint, string message, Action<EndPoint, byte[]> onTrigger = null)
        {
            byte[] buff = Encoding.UTF8.GetBytes(message);
            UDPSendContext context = new UDPSendContext() { EndPoint = endPoint, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendASCII(EndPoint endPoint, string message, Action<EndPoint, byte[]> onTrigger = null)
        {
            byte[] buff = Encoding.ASCII.GetBytes(message);
            UDPSendContext context = new UDPSendContext() { EndPoint = endPoint, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendBytes(EndPoint endPoint, byte[] buff, Action<EndPoint, byte[]> onTrigger = null)
        {
            UDPSendContext context = new UDPSendContext() { EndPoint = endPoint, Buff = buff };

            Send(context, onTrigger);
        }
        public void SendEvent(EndPoint endPoint, Action<EndPoint, byte[]> onTrigger = null)
        {
            UDPSendContext context = new UDPSendContext() { EndPoint = endPoint, Buff = null };

            Send(context, onTrigger);
        }
        protected override void Send(UDPSendContext context, Action<EndPoint, byte[]> onTrigger)
        {
            try
            {
                //发送Buff
                _server.BeginSendTo(context.Buff, 0, context.Buff.Length, SocketFlags.None, context.EndPoint, new AsyncCallback((asyncSend) =>
                {
                    Socket c = (Socket)asyncSend.AsyncState;
                    c.EndSend(asyncSend);

                    MainThreadUtility.Post<EndPoint, byte[]>(onTrigger, context.EndPoint, context.Buff);
                    MainThreadUtility.Post<EndPoint, byte[]>(OnSend, context.EndPoint, context.Buff);//OnSend回调
                }), _server);
            }
            catch (SocketException ex)
            {
                MLog.Print(ex);
            }
        }
        protected override void Send(UDPSendContext context, Action<EndPoint, UDPDataPack> onTrigger = null) 
        {
            throw new NotSupportedException();
        }
    }
}
