using System;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// Manager脚本用
    /// </summary>
    [NoComponent]
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private const string k_RootName = "#MonoSingletons#";//父级，所有的MonoSingleton脚本都会在该GameObject名下
        private static T sm_Instance;

        //注意:静态构造函数的调用时机为:
        //在调用静态成员之前或者在创建实例之前
        //这意味着对于继承MonoSingleton的类必须通过调用Instance来触发创建
        static MonoSingleton()
        {
            //尝试获取父物体#MonoSingleton#
            GameObject rootObj = GameObject.Find(k_RootName);
            //对于第一个MonoSingleton脚本，此时还没有创建，需要创建
            if (rootObj == null)
            {
                rootObj = new GameObject(k_RootName);
                DontDestroyOnLoad(rootObj);
            }

            Transform rootTransform = rootObj.transform;
            Type type = typeof(T);
            string name = type.Name;//默认类名
            HideFlags hideFlags = HideFlags.None;//默认HideFlags
#if UNITY_EDITOR
            //在Editor中，可以通过[MonoSingletonSetting]特性获取更改的参数(HideFlags/Name)
            var attributes = type.GetCustomAttributes(true);
            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i] is MonoSingletonSetting mss)
                {
                    hideFlags = mss.hideFlags;
                    name = !string.IsNullOrEmpty(mss.nameInHierarchy) ? mss.nameInHierarchy : name;
                    break;
                }
            }
#endif
            //最终设置
            sm_Instance = new GameObject(name).AddComponent<T>();//创建唯一实例在名为name的GameObject上
            sm_Instance.transform.SetParent(rootTransform, false);
            sm_Instance.hideFlags = hideFlags;
        }

        public static T Instance
        {
            get
            {
                return sm_Instance;
            }
        }

        protected virtual void OnDestroy()
        {
            sm_Instance = null;
        }
    }

    /// <summary>
    /// 该类用于更改两个设置：HideFlags和名字
    /// 特性会通过反射获取这两个属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MonoSingletonSetting : Attribute
    {
        private HideFlags m_HideFlags;
        private string m_NameInHierarchy;//null或""时默认使用类名

        public MonoSingletonSetting(HideFlags hideFlags, string nameInHierarchy = null)
        {
            m_HideFlags = hideFlags;
            m_NameInHierarchy = nameInHierarchy;
        }

        public string nameInHierarchy
        {
            get { return m_NameInHierarchy; }
        }

        public HideFlags hideFlags
        {
            get { return m_HideFlags; }
        }
    }
}