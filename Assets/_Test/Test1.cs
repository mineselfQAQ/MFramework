using MFramework;
using MFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    private void Start()
    {
        A a = new A();
        A b = new A();
        a.a = 10;
        b.a = 5;
        //Debug.Log(a == b);
        Debug.Log(a.Equals(b));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log(MyClass.X);
        }
    }
}

public class MyClass
{
    public static int X = InitializeX();

    private static int InitializeX()
    {
        Debug.Log("Static field X initialized");
        return 100;
    }
}

public struct A
{
    public int a;
}