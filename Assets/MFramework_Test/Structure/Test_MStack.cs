using MFramework;
using UnityEngine;

public class Test_MStack : MonoBehaviour
{
    private void Start()
    {
        MStack stack = new MStack();
        stack.Push(1);//1
        stack.Push(2);//2 1
        stack.Push(3);//3 2 1

        stack.Print();

        Log.Print(Log.ColorWord("---�ָ���---", Color.red));

        stack.Pop();//2 1
        stack.Print();

        Log.Print(Log.ColorWord("---�ָ���---", Color.red));

        Debug.Log("ջ��: " + stack.Peek());
        Debug.Log("�Ƿ����3: " + stack.Contains(3));

        Log.Print(Log.ColorWord("---�ָ���---", Color.red));

        stack.Clear();
        stack.Print();
    }
}
