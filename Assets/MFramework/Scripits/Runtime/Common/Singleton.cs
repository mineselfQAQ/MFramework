using System.Reflection;
using System;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// 标准Singleton
    /// </summary>
    public abstract class Singleton<T> where T : Singleton<T>
    {
        private static T sm_Instance;

        static Singleton()
        {
            //获取T的所有构造函数
            var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            //找到参数列表为空的那个私有构造函数
            var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
            //没找到报错
            if (ctor == null) { throw new Exception($"|{typeof(T).ToString()}|类中不存在私有无参构造函数"); }
            //创建实例
            sm_Instance = ctor.Invoke(null) as T;
        }

        static public T Instance { get { return sm_Instance; } }

        protected Singleton() { }
    }
}
