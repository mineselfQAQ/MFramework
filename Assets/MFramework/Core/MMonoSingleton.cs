
using UnityEngine;

namespace MFramework.Core
{
    // TODO：AI生成
    [DisallowMultipleComponent]
    public abstract class MMonoSingleton<T> : MonoBehaviour where T : MMonoSingleton<T>
    {
        [SerializeField] private bool _dontDestroyOnLoad = true;

        private static readonly object _instanceLock = new object();
        private static T _instance;
        private static bool _isQuitting;

        public static bool HasInstance => _instance != null;

        public static T Instance
        {
            get
            {
                if (_isQuitting) return null;
                if (_instance != null) return _instance;

                lock (_instanceLock)
                {
                    if (_instance != null) return _instance;

                    _instance = FindFirstObjectByType<T>();
                    if (_instance != null) return _instance;

                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                MLog.Default?.W($"{typeof(T).Name} 已存在实例，销毁重复对象：{name}");
                Destroy(gameObject);
                return;
            }

            _instance = (T)this;
            _isQuitting = false;

            if (_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
