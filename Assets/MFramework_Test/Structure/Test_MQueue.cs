using MFramework;
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

        Log.Print(Log.ColorWord("---分隔符---", Color.red));

        queue.Dequeue();
        queue.Print();
        
        Log.Print(Log.ColorWord("---分隔符---", Color.red));

        Log.Print("队首: " + queue.Peek());
        Log.Print("是否包含1: " + queue.Contains(1));

        Log.Print(Log.ColorWord("---分隔符---", Color.red));

        queue.Clear();
        queue.Print();
    }
}
