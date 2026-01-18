using System;
using MFramework.Core.Event;

namespace MFramework.Core
{
    public interface ITrackerEvent : IEvent
    {
        MTracker Tracker { get; }
        DateTime EventTime { get; }
    }
    
    public class TrackerStartedEvent : ITrackerEvent
    {
        public MTracker Tracker { get; }
        public DateTime EventTime { get; }
        
        public TrackerStartedEvent(MTracker tracker)
        {
            Tracker = tracker;
            EventTime = DateTime.Now;
        }
    }

    public class TrackerStoppedEvent : ITrackerEvent
    {
        public MTracker Tracker { get; }
        public DateTime EventTime { get; }
        
        public TrackerStoppedEvent(MTracker tracker)
        {
            Tracker = tracker;
            EventTime = DateTime.Now;
        }
    }
}