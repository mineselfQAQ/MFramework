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

        MLog.Print(MLog.Color("---分隔符---", Color.red));

        queue.PopFirst();
        queue.PopLast();
        queue.Print();

        MLog.Print(MLog.Color("---分隔符---", Color.red));

        MLog.Print("队首: " + queue.PeekFirst());
        MLog.Print("是否包含1: " + queue.Contains(1));

        MLog.Print(MLog.Color("---分隔符---", Color.red));

        queue.Clear();
        queue.Print();
    }
}
