using System;
using UnityEngine;

public class Test_Event2 : MonoBehaviour
{
    private void Start()
    {
        Publisher2 publisher = new Publisher2();
        Subscriber2 subscriber = new Subscriber2();

        Publisher2.eventHandler += Subscriber2.Print;
        publisher.Execute();
    }
}

class Publisher2
{
    public static int sNum = 5;
    public int num = 10;

    public static event EventHandler<Subscriber2EventArgs> eventHandler;
    public void Execute()
    {
        Subscriber2EventArgs args = new Subscriber2EventArgs();
        args.num = num;
        eventHandler(this, args);
    }
}

class Subscriber2
{
    public static void Print(object sender, Subscriber2EventArgs e)
    {
        Debug.Log(sender);
        Debug.Log(e.num);
    }
}

class Subscriber2EventArgs : EventArgs
{
     public int num { get; set; }
}
