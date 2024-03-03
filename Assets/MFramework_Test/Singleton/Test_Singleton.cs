using MFramework.Test;
using UnityEngine;

public class Test_Singleton : MonoBehaviour
{
    void Start()
    {
        //Singleton<>总结：
        //如果想要让某类为单例的，那么就将其继承自Singleton<类名>即可
        //有一些注意点：
        //1.类中必须有私有构造函数，Singleton<>中会通过反射创建实例(必须找到才能调用)
        //2.如下方例子所示，SingletonClass2类会得到SingletonClass类的实例，这是不正确的，需要避免
        SingletonClass.Instance.Print();
        SingletonClass2.Instance.Print();//内部报错，没有私有构造函数

        //MonoSingleton<>总结：
        //如果想要在场景中得到唯一实例，那么就将其继承自MonoSingleton<类名>即可
        //为了在编辑器下获得更好的显示，需要使用[MonoSingletonSetting]特性
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
