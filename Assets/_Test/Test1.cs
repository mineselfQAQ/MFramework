using MFramework;
using MFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    public int time;
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
        var c = GetComponent<CharacterController>();
        c.Move(new Vector3(-Time.deltaTime * time, 0, 0));
    }
}

public class A
{
    public int a;

    public virtual void INN()
    {

    }
}

public class B : A
{

    public sealed override void INN()
    {

    }
}