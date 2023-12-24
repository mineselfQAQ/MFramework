using UnityEngine;

delegate void MyDel();

delegate int MyDel2();

delegate string MyDel3(int a, int b);

public class Test_Delegate : MonoBehaviour
{
    private void Start()
    {
        //===MyDel()===
        MyDel myDel = () =>
        {
            Debug.Log("无参无返回值Delegate");
        };
        myDel += () => { Debug.Log("追加"); };

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
}
