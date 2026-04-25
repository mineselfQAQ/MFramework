using MFramework.Core.Event;

namespace MFramework.Event
{
    public abstract class UnityLifecycleEvent : IEvent
    {
        protected UnityLifecycleEvent(UnityLifecycleEventDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        public UnityLifecycleEventDispatcher Dispatcher { get; }
    }

    public sealed class UnityUpdateEvent : UnityLifecycleEvent
    {
        public UnityUpdateEvent(UnityLifecycleEventDispatcher dispatcher) : base(dispatcher)
        {
        }
    }

    public sealed class UnityFixedUpdateEvent : UnityLifecycleEvent
    {
        public UnityFixedUpdateEvent(UnityLifecycleEventDispatcher dispatcher) : base(dispatcher)
        {
        }
    }

    public sealed class UnityLateUpdateEvent : UnityLifecycleEvent
    {
        public UnityLateUpdateEvent(UnityLifecycleEventDispatcher dispatcher) : base(dispatcher)
        {
        }
    }

    public sealed class UnityApplicationFocusEvent : UnityLifecycleEvent
    {
        public UnityApplicationFocusEvent(UnityLifecycleEventDispatcher dispatcher, bool hasFocus) : base(dispatcher)
        {
            HasFocus = hasFocus;
        }

        public bool HasFocus { get; }
    }

    public sealed class UnityApplicationPauseEvent : UnityLifecycleEvent
    {
        public UnityApplicationPauseEvent(UnityLifecycleEventDispatcher dispatcher, bool pauseStatus) : base(dispatcher)
        {
            PauseStatus = pauseStatus;
        }

        public bool PauseStatus { get; }
    }

    public sealed class UnityApplicationQuitEvent : UnityLifecycleEvent
    {
        public UnityApplicationQuitEvent(UnityLifecycleEventDispatcher dispatcher) : base(dispatcher)
        {
        }
    }
}
