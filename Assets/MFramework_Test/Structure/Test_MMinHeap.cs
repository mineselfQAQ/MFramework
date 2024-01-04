using MFramework;
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
        Log.Print("总数: " + heap.Count);

        Log.Print("弹出堆顶元素: " + heap.Pop());
        Log.Print("此时堆顶元素: " + heap.Peek());

        Log.Print("弹出堆顶元素: " + heap.Pop());
        Log.Print("此时堆顶元素: " + heap.Peek());

        Log.Print("总数: " + heap.Count);

        Log.Print(Log.ColorWord("---分隔符---", Color.red));

        MList<int> list = new MList<int>() { 3, 5, 8, 4, 7 };
        MMinHeap<int> heap2 = new MMinHeap<int>(list);
        Log.Print("总数: " + heap2.Count);

        Log.Print("弹出堆顶元素: " + heap2.Pop());
        Log.Print("此时堆顶元素: " + heap2.Peek());

        Log.Print("弹出堆顶元素: " + heap2.Pop());
        Log.Print("此时堆顶元素: " + heap2.Peek());

        Log.Print("总数: " + heap2.Count);
    }
}
