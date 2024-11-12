using MFramework;
using UnityEngine;

public class Test_GenericConstraint : MonoBehaviour
{
    private void Start()
    {
        AddClassFactory<AddClass> factory = new AddClassFactory<AddClass>();
        var instance = factory.CreateInstance(1, 4);
        MLog.Print(instance.Add());
    }

    private class AddClass : INew
    {
        private int a;
        private int b;

        //等价与构造函数，使用NewFactory<>创建出来的实例必然会调用
        public void Init(int a, int b)
        {
            this.a = a;
            this.b = b;
        }

        //原本应该是该构造函数，但是没有泛型约束无法做到
        //public AddClass(int a, int b)
        //{
        //    this.a = a;
        //    this.b = b;
        //}

        public int Add()
        {
            return a + b;
        }
    }

    public interface INew
    {
        void Init(int a, int b);
    }

    //即限定T类型具有Inew.Init()的构造函数
    public class AddClassFactory<T> where T : INew, new()
    {
        public T CreateInstance(int a, int b)
        {
            T instance = new T();
            instance.Init(a, b);
            return instance;
        }
    }
}
