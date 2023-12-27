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

        Log.Print(Log.ColorWord("---分隔符---", Color.red));

        queue.PopFirst();
        queue.PopLast();
        queue.Print();

        Log.Print(Log.ColorWord("---分隔符---", Color.red));

        Log.Print("队首: " + queue.PeekFirst());
        Log.Print("是否包含1: " + queue.Contains(1));

        Log.Print(Log.ColorWord("---分隔符---", Color.red));

        queue.Clear();
        queue.Print();
    }
}
