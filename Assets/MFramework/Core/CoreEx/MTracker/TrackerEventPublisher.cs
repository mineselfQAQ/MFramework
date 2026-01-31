using MFramework.Core.Event;

namespace MFramework.Core
{
    public interface ITrackerEventPublisher
    {
        MEventBus EventBus { get; }
        void Publish<T>(T trackerEvent) where T : ITrackerEvent;
    }

    /// <summary>
    /// 派发类，需自行使用EventBus进行Register
    /// </summary>
    public class TrackerEventPublisher : ITrackerEventPublisher
    {
        public MEventBus EventBus { get; }

        public TrackerEventPublisher(MEventBus eventBus)
        {
            EventBus = eventBus;
        }

        public void Publish<T>(T trackerEvent) where T : ITrackerEvent
        {
            EventBus.Publish<T>(trackerEvent);
        }
    }
}