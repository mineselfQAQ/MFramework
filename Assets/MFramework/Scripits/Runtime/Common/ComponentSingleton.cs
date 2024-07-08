using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// 被挂载在GameObject上的组件的MonoBehaviour脚本用
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class ComponentSingleton<T> : MonoBehaviour where T : ComponentSingleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)//用于新场景新组件
            {
                Destroy(gameObject);
            }
            else//初次实例化
            {
                T[] objects = FindObjectsOfType<T>();
                if (objects.Length > 1)
                {
                    MLog.Print($"{typeof(ComponentSingleton<T>)}：当前存在{objects.Length}个{typeof(T)}脚本，请检查", MLogType.Error);
                }

                Instance = objects[0];
            }
        }
    }
}