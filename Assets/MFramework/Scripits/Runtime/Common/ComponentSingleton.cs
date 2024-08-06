using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// 被挂载在GameObject上的组件的MonoBehaviour脚本用
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class ComponentSingleton<T> : MonoBehaviour where T : ComponentSingleton<T>
    {
        protected static T m_instance;

        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    T[] objects = FindObjectsOfType<T>();
                    if (objects.Length > 1)
                    {
                        MLog.Print($"{typeof(ComponentSingleton<T>)}：当前存在{objects.Length}个{typeof(T)}脚本，请检查", MLogType.Error);
                    }
                    else if (objects.Length == 0)
                    {
                        MLog.Print($"{typeof(ComponentSingleton<T>)}：未挂载ComponentSingleton<{typeof(T)}>脚本，请检查", MLogType.Error);
                    }

                    m_instance = objects[0];
                }

                return m_instance;
            }
        }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)//用于新场景新组件
            {
                DestroyImmediate(gameObject);
                if (typeof(T) == typeof(MCore)) return;//MCore不需要提示

                MLog.Print($"{typeof(ComponentSingleton<T>)}：{typeof(T)}作为ComponentSingleton脚本再次挂载，请检查是否正确", MLogType.Warning);
            }
        }
    }
}