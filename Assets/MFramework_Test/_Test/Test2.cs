using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AAA;
using AAA.BB;

public class Test2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        A a = new A();
        B b = new B();

        C c = new C();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

namespace AAA.BB
{
    public class C
    {
        
    }
}