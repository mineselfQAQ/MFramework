using MFramework.Test;
using UnityEngine;

public class Test_Singleton : MonoBehaviour
{
    void Start()
    {
        SingletonClass.Instance.Print();
        SingletonClass2.Instance.Print();//内部报错，没有私有构造函数
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
            Log.Print($"{x}, {y}");
        }
    }

    public class SingletonClass2 : Singleton<SingletonClass>
    {
        private int x;
        private int y;

        public void Print()
        {
            Log.Print($"{x}, {y}");
        }
    }

    [MonoSingletonSetting(HideFlags.NotEditable, "#MonoClass#")]
    public class MonoClass : MonoSingleton<MonoClass>
    {
        public void Print()
        {
            Log.Print("OK");
        }
    }

}
