using MFramework;
using System.Collections.Generic;
using UnityEngine;

public class Test_MQueue : MonoBehaviour
{
    private void Start()
    {
        MQueue queue = new MQueue();
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);
        queue.Print();

        MLog.Print(MLog.ColorWord("---�ָ���---", Color.red));

        queue.Dequeue();
        queue.Print();
        
        MLog.Print(MLog.ColorWord("---�ָ���---", Color.red));

        MLog.Print("����: " + queue.Peek());
        MLog.Print("�Ƿ����1: " + queue.Contains(1));

        MLog.Print(MLog.ColorWord("---�ָ���---", Color.red));

        queue.Clear();
        queue.Print();
    }
}
