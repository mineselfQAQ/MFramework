using System;
using System.Collections;
using UnityEngine;
using MFramework;

public class Test_IComparer : MonoBehaviour
{
    private void Start()
    {
        A a1 = new A("a1", 12);
        A a2 = new A("a2", 5);
        A a3 = new A("a3", 5);

        MLog.Print($"{a1.name}: {a1.num}  {a2.name}: {a2.num}  {a3.name}: {a3.num}");
        Compare(a1, a2);
        Compare(a2, a1);
        Compare(a2, a3);
    }

    public void Compare(A a, A b)
    {
        int num = A.Default.Compare(a, b);

        if (num < 0)
        {
            MLog.Print($"{a.name} < {b.name}");
        }
        else if (num == 0)
        {
            MLog.Print($"{a.name} = {b.name}");
        }
        else if (num > 0)
        {
            MLog.Print($"{a.name} > {b.name}");
        }
    }

    public class A : IComparer
    {
        public static IComparer Default = new A();

        public string name;
        public int num;

        public A() { }
        public A(string name, int num)
        {
            this.name = name;
            this.num = num;
        }

        int IComparer.Compare(object x, object y)
        {
            A a = x as A;
            if (a == null) throw new Exception();
            A b = y as A;
            if (b == null) throw new Exception();

            if (a.num > b.num)
            {
                return 1;
            }
            else if (a.num == b.num)
            {
                return 0;
            }
            else if (a.num < b.num)
            {
                return -1;
            }

            throw new Exception();
        }
    }
}

   