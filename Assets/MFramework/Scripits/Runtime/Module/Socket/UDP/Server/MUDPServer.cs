using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEditor.PackageManager;
using UnityEngine;

namespace MFramework
{
    public class MUDPServer
    {
        public string IP;//������IP
        public int Port;//������Port

        private Socket _server;
        private List<EndPoint> _clients;//�����ϵĿͻ���
        private Dictionary<EndPoint, DataBuffer> _bufferDic;//�ͻ��˻����ֵ�
        private EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);//����ͻ���EndPoint

        public MUDPServer(string ip, int port)
        {
            IP = ip;
            Port = port;

            _clients = new List<EndPoint>();
            _bufferDic = new Dictionary<EndPoint, DataBuffer>();

            _server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _server.Bind(new IPEndPoint(IPAddress.Parse(ip), port));

            MLog.Print($"{typeof(MUDPServer)}��������<{_server.LocalEndPoint}>�ѿ�ʼ����");

            ReceiveData();
        }

        public void Quit()
        {
            _server.Close();
        }

        private void ReceiveData()
        {
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
                    if (!_clients.Contains(endPoint))//���Ӵ���
                    {
                        byte[] verificationBytes = new byte[4] { 18, 203, 59, 38 };
                        byte[] receviedBytes = new byte[4];
                        Array.Copy(bytes, 0, receviedBytes, 0, 4);
                        if (receviedBytes.SequenceEqual(verificationBytes))//��֤ͨ��
                        {
                            //�ͻ������ӻ�Ӧ
                            byte[] buff = new byte[1] { 1 };
                            _server.SendTo(buff, endPoint);

                            _clients.Add(endPoint);
                            MLog.Print($"{typeof(MUDPServer)}���ͻ���<{endPoint}>������");
                        }
                    }
                    else//һ�㴦��
                    {
                        //��ʼ��δ������DataBuffer
                        if (!_bufferDic.ContainsKey(endPoint))
                        {
                            _bufferDic[endPoint] = new DataBuffer();
                        }
                        //���ݼ��뻺������(���ݿ��ܷ�������Ҳ����ͬʱ������)
                        _bufferDic[endPoint].AddBuffer(bytes, len);
                        //��ȡ����(�����ȡ)
                        var dataPack = new SocketDataPack();
                        if (_bufferDic[endPoint].TryUnpack(out dataPack))
                        {
                            Debug.Log($"�յ����Կͻ���<{endPoint}>����Ϣ��{dataPack.ToString()}");
                        }
                    }
                }

                //������������
                ReceiveData();
            }
            catch (Exception e)
            {
                MLog.Print("���ݽ���ʧ�ܣ�" + e.Message, MLogType.Warning);
            }
        }

        public void Send(EndPoint endPoint, UInt16 e, byte[] buff = null)
        {
            //��ɰ���ȡ��Buff
            buff = buff ?? new byte[] { };
            var dataPack = new SocketDataPack(e, buff);
            var data = dataPack.Buff;

            try
            {
                //����Buff
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
