using System;
using MFramework.Core;
using MFramework.Core.CoreEx;
using MFramework.Core.Event;
using MFramework.Core.IOC;
using MFramework.Core.Tracker;

namespace MFrameworkExamples.Event
{
    public class TestEvent : IEvent
    {
        private string _message;
        public string Message => _message;

        public TestEvent(string message)
        {
            _message = message;
        }
    }

    public class MEntry : MEntryBase
    {
        private MEventBus _eventBus = new MEventBus();

        protected override IManagedService[] ConfigureServices()
        {
            return null;
        }

        protected override void OnBootstrapped(TrackerStoppedEvent e)
        {
            _eventBus.LogError = (message) =>
            {
                MLog.Default.E(message);
            };

            // IEvent版
            _eventBus.RegisterSafe<TestEvent>((e) => throw new Exception(e.Message));
            _eventBus.Publish(new TestEvent("IEvent版错误"));

            // Name版
            _eventBus.RegisterSafe("EventByName", () => throw new Exception("Name版错误"));
            _eventBus.Publish("EventByName");
        }
    }
}
