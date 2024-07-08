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

        //ע�⣺�����иú������е��ã���Ϊ�¼�ֻ�����Լ������е���
        public static void Execute()
        {
            mydel();
        }
    }

    class Subscriber
    {
        public static void Print()
        {
            Debug.Log("Subscriber�ඩ����");
        }
    }
}