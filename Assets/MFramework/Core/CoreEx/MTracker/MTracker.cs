using System;
using System.Collections.Generic;

namespace MFramework.Core
{
    public interface IMTracker
    {
        int Id { get; }
        string Name { get; }
        
        DateTime StartTime { get; }
        DateTime EndTime { get; }
        TimeSpan Duration { get; }
        
        bool IsStarted { get; }
        bool IsRunning { get; }
        bool IsCompleted { get; }
        
        void Start();
        void Stop();
    }
    
    public class MTracker : IMTracker
    {
        private int _id;
        private string _name;
        private bool _isStarted;
        private bool _isCompleted;

        private readonly ITrackerEventPublisher _publisher;
        private readonly ITrackerCollector _collector;

        public int Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }
        
        public bool IsStarted
        {
            get { return _isStarted; }
        }
        
        public bool IsCompleted
        {
            get { return _isCompleted; }
        }
        
        public bool IsRunning => _isStarted && !_isCompleted;
        
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        public TimeSpan Duration => _isCompleted ? EndTime - StartTime : TimeSpan.Zero;
        
        public MTracker(int id, string name, ITrackerEventPublisher publisher, ITrackerCollector collector)
        {
            _id = id;
            _name = name;
            _publisher = publisher;
            _collector = collector;
        }

        public static MAutoTracker StartNew(MTracker tracker)
        {
            tracker.Start();
            return new MAutoTracker(tracker);
        }

        public void Start()
        {
            if (_isStarted) return;
            
            StartTime = DateTime.Now;
            _isStarted = true;
            
            _publisher?.Publish(new TrackerStartedEvent(this));
        }
        
        public void Stop()
        {
            if(!_isStarted || _isCompleted) return;
            
            EndTime = DateTime.Now;
            _isCompleted = true;
                
            _publisher?.Publish(new TrackerStoppedEvent(this));
        }

        public bool TryGetCollector(out ITrackerCollector collector)
        {
            collector = _collector;
            return _collector != null;
        }
        
        public override string ToString()
        {
            if (!_isCompleted) return "未完成";
            
            var strs = new List<string>();
            var tp = Duration;
    
            strs.Add($"{_id}-{_name}计时：");
            if (tp.Days > 0) strs.Add($"{tp.Days}天");
            if (tp.Hours > 0 || strs.Count > 0) strs.Add($"{tp.Hours}时");
            if (tp.Minutes > 0 || strs.Count > 0) strs.Add($"{tp.Minutes}分");
            strs.Add($"{tp.Seconds}秒");
            strs.Add($"{tp.Milliseconds:000}毫秒");
    
            return string.Join("", strs);
        }
    }

    public static class MTrackerFactory
    {
        public static MTracker CreateTracker(int id, string name)
        {
            return new MTracker(id, name, null, null);
        }
        
        public static MTracker CreateTracker(int id, string name, ITrackerEventPublisher publisher)
        {
            return new MTracker(id, name, publisher, null);
        }
        
        public static MTracker CreateTracker(int id, string name, ITrackerCollector collector)
        {
            return new MTracker(id, name, null, collector);
        }
        
        public static MTracker CreateTracker(int id, string name, ITrackerEventPublisher publisher, ITrackerCollector collector)
        {
            return new MTracker(id, name, publisher, collector);
        }
    }

    public class MAutoTracker : IDisposable
    {
        private readonly MTracker _tracker;
        public MAutoTracker(MTracker tracker) => _tracker = tracker;
        public MTracker Tracker => _tracker;

        public void Dispose()
        {
            _tracker.Stop();
            if (_tracker.TryGetCollector(out var collector))
            {
                collector.Collect(_tracker.ToString());
            }
        }
    }
}