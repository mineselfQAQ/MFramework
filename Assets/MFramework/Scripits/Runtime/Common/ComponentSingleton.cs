using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// ��������GameObject�ϵ������MonoBehaviour�ű���
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class ComponentSingleton<T> : MonoBehaviour where T : ComponentSingleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)//�����³��������
            {
                Destroy(gameObject);
            }
            else//����ʵ����
            {
                T[] objects = FindObjectsOfType<T>();
                if (objects.Length > 1)
                {
                    MLog.Print($"{typeof(ComponentSingleton<T>)}����ǰ����{objects.Length}��{typeof(T)}�ű�������", MLogType.Error);
                }

                Instance = objects[0];
            }
        }
    }
}