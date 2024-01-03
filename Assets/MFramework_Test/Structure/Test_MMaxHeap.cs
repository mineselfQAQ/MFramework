using MFramework;
using UnityEngine;

public class Test_MMaxHeap : MonoBehaviour
{
    private void Start()
    {
        MMaxHeap<int> heap = new MMaxHeap<int>();

        heap.Push(3);
        heap.Push(1);
        heap.Push(5);
        heap.Push(2);

        Log.Print(heap.Peek());
    }
}
