using MFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    protected void Awake()
    {
        A a = new A();
        A a2 = a;
        Debug.Log(a.Equals(a2));
    }

    private void Update()
    {

    }

    public class A
    {
        public A()
        {
            Debug.Log("A");
        }
    }
}