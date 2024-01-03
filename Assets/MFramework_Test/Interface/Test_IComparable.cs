using System;
using UnityEngine;
using MFramework.IComparableTest;
using MFramework;

public class Test_IComparable : MonoBehaviour
{
    private void Start()
    {
        A a1 = new A("a1", 12);
        A a2 = new A("a2", 5);
        A a3 = new A("a3", 5);

        Log.Print($"{a1.name}: {a1.num}  {a2.name}: {a2.num}  {a3.name}: {a3.num}");
        Compare(a1, a2);
        Compare(a2, a1);
        Compare(a2, a3);
    }

    private void Compare(A a, A b)
    {
        int num = a.CompareTo(b);

        if (num < 0)
        {
            Log.Print($"{a.name} < {b.name}");
        }
        else if (num == 0)
        {
            Log.Print($"{a.name} = {b.name}");
        }
        else if (num > 0)
        {
            Log.Print($"{a.name} > {b.name}");
        }
    }
}

namespace MFramework.IComparableTest
{
    public class A : IComparable
    {
        public string name;
        public int num;

        public A(string name, int num)
        {
            this.name = name;
            this.num = num;
        }

        public int CompareTo(object obj)
        {
            A temp = obj as A;
            if (temp == null) throw new Exception();

            int num2 = temp.num;

            if (num < num2)
            {
                return -1;
            }
            else if (num == num2)
            {
                return 0;
            }
            else if(num > num2)
            {
                return 1;
            }

            throw new Exception();
        }
    }
}