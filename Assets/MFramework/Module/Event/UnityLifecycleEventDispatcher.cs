using MFramework.Core.Event;
using UnityEngine;

namespace MFramework.Event
{
    public class UnityLifecycleEventDispatcher : MonoBehaviour
    {
        private MEventBus _eventBus;

        public void Initialize(MEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void Awake()
        {
            Publish(new UnityAwakeEvent(this));
        }

        private void Start()
        {
            Publish(new UnityStartEvent(this));
        }

        private void Update()
        {
            Publish(new UnityUpdateEvent(this));
        }

        private void FixedUpdate()
        {
            Publish(new UnityFixedUpdateEvent(this));
        }

        private void LateUpdate()
        {
            Publish(new UnityLateUpdateEvent(this));
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            Publish(new UnityApplicationFocusEvent(this, hasFocus));
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            Publish(new UnityApplicationPauseEvent(this, pauseStatus));
        }

        private void OnApplicationQuit()
        {
            Publish(new UnityApplicationQuitEvent(this));
        }

        private void Publish<TEvent>(TEvent eventData) where TEvent : IEvent
        {
            _eventBus?.Publish(eventData);
        }
    }
}
