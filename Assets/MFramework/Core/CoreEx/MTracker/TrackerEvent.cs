using System;
using MFramework.Core.Event;

namespace MFramework.Core
{
    public interface ITrackerEvent : IEvent
    {
        string Name { get; }
    }
    
    public class TrackerStartedEvent : ITrackerEvent
    {
        public string Name { get; }
        
        public DateTime StartTime { get; }
        
        public TrackerStartedEvent(MTracker tracker)
        {
            Name = tracker.Name;
            StartTime = tracker.StartTime;
        }
    }

    public class TrackerStoppedEvent : ITrackerEvent
    {
        public string Name { get; }
        
        public DateTime StartTime { get; }
        
        public DateTime EndTime { get; }
        
        public TimeSpan Duration { get; }
            
        public TrackerStoppedEvent(MTracker tracker)
        {
            Name = tracker.Name;
            StartTime = tracker.StartTime;
            EndTime = tracker.EndTime;
            Duration = tracker.Duration;
        }
    }
}