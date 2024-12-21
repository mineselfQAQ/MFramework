using MFramework;
using MFramework.DLC;
using UnityEngine;

public class Test_MDeque : MonoBehaviour
{
    private void Start()
    {
        MDeque queue = new MDeque();
        queue.PushLast(1);
        queue.PushLast(2);
        queue.PushLast(3);
        queue.PushFirst(4);
        queue.Print();

        MLog.Print(MLog.Color("---�ָ���---", Color.red));

        queue.PopFirst();
        queue.PopLast();
        queue.Print();

        MLog.Print(MLog.Color("---�ָ���---", Color.red));

        MLog.Print("����: " + queue.PeekFirst());
        MLog.Print("�Ƿ����1: " + queue.Contains(1));

        MLog.Print(MLog.Color("---�ָ���---", Color.red));

        queue.Clear();
        queue.Print();
    }
}
