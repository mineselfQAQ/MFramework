using MFramework;
using MFramework.DLC;
using UnityEngine;

public class Test_MMinHeap : MonoBehaviour
{
    private void Start()
    {
        MMinHeap<int> heap = new MMinHeap<int>();

        heap.Push(3);
        heap.Push(5);
        heap.Push(8);
        heap.Push(4);
        heap.Push(7);
        MLog.Print("����: " + heap.Count);

        MLog.Print("�����Ѷ�Ԫ��: " + heap.Pop());
        MLog.Print("��ʱ�Ѷ�Ԫ��: " + heap.Peek());

        MLog.Print("�����Ѷ�Ԫ��: " + heap.Pop());
        MLog.Print("��ʱ�Ѷ�Ԫ��: " + heap.Peek());

        MLog.Print("����: " + heap.Count);

        MLog.Print(MLog.ColorWord("---�ָ���---", Color.red));

        MList<int> list = new MList<int>() { 3, 5, 8, 4, 7 };
        MMinHeap<int> heap2 = new MMinHeap<int>(list);
        MLog.Print("����: " + heap2.Count);

        MLog.Print("�����Ѷ�Ԫ��: " + heap2.Pop());
        MLog.Print("��ʱ�Ѷ�Ԫ��: " + heap2.Peek());

        MLog.Print("�����Ѷ�Ԫ��: " + heap2.Pop());
        MLog.Print("��ʱ�Ѷ�Ԫ��: " + heap2.Peek());

        MLog.Print("����: " + heap2.Count);
    }
}
