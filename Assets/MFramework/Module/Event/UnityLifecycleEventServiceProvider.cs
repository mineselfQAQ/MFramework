using MFramework.Core.CoreEx;
using MFramework.Core.Event;
using MFramework.Core.IOC;

using UnityEngine;

namespace MFramework.Event
{
    public interface IUnityLifecycleEventServiceProvider
    {
        MEventBus EventBus { get; }
    }

    public class UnityLifecycleEventServiceProvider : IManagedService, IUnityLifecycleEventServiceProvider
    {
        private const string NAME = "MFramework_UnityLifecycleEventDispatcher";

        private readonly bool _dontDestroyOnLoad;
        private GameObject _dispatcherObject;
        private UnityLifecycleEventDispatcher _dispatcher;

        public UnityLifecycleEventServiceProvider(MEventBus eventBus = null, bool dontDestroyOnLoad = true)
        {
            EventBus = eventBus ?? new MEventBus();
            _dontDestroyOnLoad = dontDestroyOnLoad;
        }

        public MEventBus EventBus { get; }

        public UnityLifecycleEventDispatcher Dispatcher => _dispatcher;

        public void Register()
        {
            MIOCContainer.Default.RegisterSingleton<IUnityLifecycleEventServiceProvider>(this);
        }

        public void Initialize()
        {
            if (_dispatcher != null) return;

            _dispatcherObject = new GameObject(NAME);
            _dispatcherObject.SetActive(false);
            _dispatcher = _dispatcherObject.AddComponent<UnityLifecycleEventDispatcher>();
            _dispatcher.Initialize(EventBus);

            if (_dontDestroyOnLoad && Application.isPlaying)
            {
                Object.DontDestroyOnLoad(_dispatcherObject);
            }

            _dispatcherObject.SetActive(true);
        }

        public void Shutdown()
        {
            EventBus.Clear();

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

        public void Unregister()
        {

        }
    }
}
