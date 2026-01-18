using MFramework.Core.Event;

namespace MFramework.Core
{
    public interface ITrackerEventPublisher
    {
        EventBus EventBus { get; }
        void Publish<T>(T trackerEvent) where T : ITrackerEvent;
    }

    /// <summary>
    /// 派发类，需自行使用EventBus进行Register
    /// </summary>
    public class TrackerEventPublisher : ITrackerEventPublisher
    {
        public EventBus EventBus { get; }

        public TrackerEventPublisher(EventBus eventBus)
        {
            EventBus = eventBus;
        }

        public void Publish<T>(T trackerEvent) where T : ITrackerEvent
        {
            EventBus.Publish<T>(trackerEvent);
        }
    }
}