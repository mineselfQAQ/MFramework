using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_LiskovSubstitutionPrinciple : MonoBehaviour
{
    //https://zhuanlan.zhihu.com/p/24614363
    private void Start()
    {
        A a = new A();
        a.Sub(2, 1);

        B b = new B();
        b.Sub(2, 1);
    }

    private class A
    {
        public int Sub(int a, int b)
        {
            return a - b;
        }
    }

    private class B : A
    {
        public int Sub(int a, int b)
        {
            return a + b;
        }
    }
}
