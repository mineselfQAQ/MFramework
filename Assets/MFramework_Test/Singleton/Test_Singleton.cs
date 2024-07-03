using MFramework.Test;
using UnityEngine;

public class Test_Singleton : MonoBehaviour
{
    void Start()
    {
        //Singleton<>�ܽ᣺
        //�����Ҫ��ĳ��Ϊ�����ģ���ô�ͽ���̳���Singleton<����>����
        //��һЩע��㣺
        //1.���б�����˽�й��캯����Singleton<>�л�ͨ�����䴴��ʵ��(�����ҵ����ܵ���)
        //2.���·�������ʾ��SingletonClass2���õ�SingletonClass���ʵ�������ǲ���ȷ�ģ���Ҫ����
        SingletonClass.Instance.Print();
        SingletonClass2.Instance.Print();//�ڲ�����û��˽�й��캯��

        //MonoSingleton<>�ܽ᣺
        //�����Ҫ�ڳ����еõ�Ψһʵ������ô�ͽ���̳���MonoSingleton<����>����
        //Ϊ���ڱ༭���»�ø��õ���ʾ����Ҫʹ��[MonoSingletonSetting]����
        MonoClass.Instance.Print();
    }
}

namespace MFramework.Test
{
    public class SingletonClass : Singleton<SingletonClass>
    {
        private int x;
        private int y;

        private SingletonClass()
        {
            x = 10;
            y = 20;
        }

        public void Print()
        {
            MLog.Print($"{x}, {y}");
        }
    }

    public class SingletonClass2 : Singleton<SingletonClass>
    {
        private int x;
        private int y;

        public void Print()
        {
            MLog.Print($"{x}, {y}");
        }
    }

    [MonoSingletonSetting(HideFlags.NotEditable, "#MonoClass#")]
    public class MonoClass : MonoSingleton<MonoClass>
    {
        public void Print()
        {
            MLog.Print("OK");
        }
    }
}
