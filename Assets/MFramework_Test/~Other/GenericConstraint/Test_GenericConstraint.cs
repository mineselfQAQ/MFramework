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

        //�ȼ��빹�캯����ʹ��NewFactory<>����������ʵ����Ȼ�����
        public void Init(int a, int b)
        {
            this.a = a;
            this.b = b;
        }

        //ԭ��Ӧ���Ǹù��캯��������û�з���Լ���޷�����
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

    //���޶�T���;���Inew.Init()�Ĺ��캯��
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
