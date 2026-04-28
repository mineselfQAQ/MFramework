using MFramework.Core.Event;

namespace MFramework.Core.Tracker
{
    public interface ITrackerEventPublisher
    {
        void Publish<T>(T trackerEvent) where T : ITrackerEvent;
    }

    /// <summary>
    /// 派发类，需自行使用EventBus进行Register
    /// </summary>
    public class TrackerEventPublisher : ITrackerEventPublisher
    {
        private readonly MEventBus _eventBus;

        public TrackerEventPublisher(MEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Publish<T>(T trackerEvent) where T : ITrackerEvent
        {
            _eventBus.Publish<T>(trackerEvent);
        }
    }
}
