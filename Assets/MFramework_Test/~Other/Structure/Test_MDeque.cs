using MFramework;
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

        MLog.Print(MLog.ColorWord("---�ָ���---", Color.red));

        queue.PopFirst();
        queue.PopLast();
        queue.Print();

        MLog.Print(MLog.ColorWord("---�ָ���---", Color.red));

        MLog.Print("����: " + queue.PeekFirst());
        MLog.Print("�Ƿ����1: " + queue.Contains(1));

        MLog.Print(MLog.ColorWord("---�ָ���---", Color.red));

        queue.Clear();
        queue.Print();
    }
}
