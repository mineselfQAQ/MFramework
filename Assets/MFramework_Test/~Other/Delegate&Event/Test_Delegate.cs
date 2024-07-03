using MFramework;
using UnityEngine;

//注意：delegate是一个自定义委托类型，所以跟class是同一级别的
delegate void MyDel();
delegate int MyDel2();
delegate string MyDel3(int a, int b);

public class Test_Delegate : MonoBehaviour
{
    private void Start()
    {
        //===MyDel()===
        //便捷语法，直接将函数传入
        //MyDel myDel = () =>
        //{
        //    Debug.Log("无参无返回值Delegate");
        //};
        //一般语法，new一个对象
        MyDel myDel = new MyDel(() => 
        {
            Debug.Log("无参无返回值Delegate");
        });
        myDel += Log;

        myDel();



        //===MyDel2()===
        MyDel2 myDel2 = () => { return 1; };
        int del2Result = myDel2();
        Debug.Log($"无参int返回值delegate，输出{del2Result}");



        //===MyDel3()===
        MyDel3 myDel3 = (int a, int b) =>
        {
            return (a + b).ToString();
        };
        string del3Result = myDel3(1, 2);
        Debug.Log(del3Result);
    }

    private void Log()
    {
        MLog.Print("LOG");
    }
}
