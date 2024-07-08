using UnityEngine;

delegate void MyDel_Event();

public class Test_Event : MonoBehaviour
{
    private void Start()
    {
        Publisher.mydel += Subscriber.Print;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Publisher.Execute();
        }
    }

    class Publisher
    {
        public static event MyDel_Event mydel;

        //注意：必须有该函数进行调用，因为事件只能在自己的类中调用
        public static void Execute()
        {
            mydel();
        }
    }

    class Subscriber
    {
        public static void Print()
        {
            Debug.Log("Subscriber类订阅了");
        }
    }
}