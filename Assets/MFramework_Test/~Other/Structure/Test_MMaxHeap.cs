using MFramework;
using MFramework.DLC;
using UnityEngine;

public class Test_MMaxHeap : MonoBehaviour
{
    private void Start()
    {
        MMaxHeap<int> heap = new MMaxHeap<int>();

        heap.Push(3);
        heap.Push(5);
        heap.Push(8);
        heap.Push(4);
        heap.Push(7);
        MLog.Print("总数: " + heap.Count);

        MLog.Print("弹出堆顶元素: " + heap.Pop());
        MLog.Print("此时堆顶元素: " + heap.Peek());

        MLog.Print("弹出堆顶元素: " + heap.Pop());
        MLog.Print("此时堆顶元素: " + heap.Peek());

        MLog.Print("总数: " + heap.Count);

        MLog.Print(MLog.ColorWord("---分隔符---", Color.red));

        MList<int> list = new MList<int>() { 3, 5, 8, 4, 7 };
        MMaxHeap<int> heap2 = new MMaxHeap<int>(list);
        MLog.Print("总数: " + heap2.Count);

        MLog.Print("弹出堆顶元素: " + heap2.Pop());
        MLog.Print("此时堆顶元素: " + heap2.Peek());

        MLog.Print("弹出堆顶元素: " + heap2.Pop());
        MLog.Print("此时堆顶元素: " + heap2.Peek());

        MLog.Print("总数: " + heap2.Count);
    }
}
