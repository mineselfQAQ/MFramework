using System.Net.Sockets;
using System.Threading;

namespace MFramework
{
    //Tip��������class���������Ͳ��ܼ򵥸�ֵ����������info.HeadTime = time;û��ref�޷���ֵ
    public class ClientSocketInfo
    {
        public Socket Client;//�����ͻ���
        public Thread ReceiveThread;//�����߳�
        public long HeadTime;//������ʱ���
    }
}