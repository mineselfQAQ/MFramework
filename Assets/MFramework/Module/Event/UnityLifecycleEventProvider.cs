using MFramework.Core.CoreEx;
using MFramework.Core.Event;
using UnityEngine;

namespace MFramework.Event
{
    public class UnityLifecycleEventProvider : IServiceProvider
    {
        private const string DispatcherName = "MFramework_UnityLifecycleEventDispatcher";

        private readonly bool _dontDestroyOnLoad;
        private GameObject _dispatcherObject;
        private UnityLifecycleEventDispatcher _dispatcher;

        public UnityLifecycleEventProvider(MEventBus eventBus = null, bool dontDestroyOnLoad = true)
        {
            EventBus = eventBus ?? new MEventBus();
            _dontDestroyOnLoad = dontDestroyOnLoad;
        }

        public MEventBus EventBus { get; }

        public UnityLifecycleEventDispatcher Dispatcher => _dispatcher;

        public void Register()
        {
        }

        public void Initialize()
        {
            if (_dispatcher != null) return;

            _dispatcherObject = new GameObject(DispatcherName);
            _dispatcherObject.SetActive(false);
            _dispatcher = _dispatcherObject.AddComponent<UnityLifecycleEventDispatcher>();
            _dispatcher.Initialize(EventBus);

            if (_dontDestroyOnLoad && Application.isPlaying)
            {
                Object.DontDestroyOnLoad(_dispatcherObject);
            }

            _dispatcherObject.SetActive(true);
        }

        public void Unregister()
        {
            EventBus.Clear();
        }

        public void Shutdown()
        {
            if (_dispatcherObject == null) return;

            if (Application.isPlaying)
            {
                Object.Destroy(_dispatcherObject);
            }
            else
            {
                Object.DestroyImmediate(_dispatcherObject);
            }

            _dispatcherObject = null;
            _dispatcher = null;
        }
    }
}
