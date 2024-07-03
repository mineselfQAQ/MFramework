using MFramework;
using UnityEngine;

//ע�⣺delegate��һ���Զ���ί�����ͣ����Ը�class��ͬһ�����
delegate void MyDel();
delegate int MyDel2();
delegate string MyDel3(int a, int b);

public class Test_Delegate : MonoBehaviour
{
    private void Start()
    {
        //===MyDel()===
        //����﷨��ֱ�ӽ���������
        //MyDel myDel = () =>
        //{
        //    Debug.Log("�޲��޷���ֵDelegate");
        //};
        //һ���﷨��newһ������
        MyDel myDel = new MyDel(() => 
        {
            Debug.Log("�޲��޷���ֵDelegate");
        });
        myDel += Log;

        myDel();



        //===MyDel2()===
        MyDel2 myDel2 = () => { return 1; };
        int del2Result = myDel2();
        Debug.Log($"�޲�int����ֵdelegate�����{del2Result}");



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
