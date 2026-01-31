using System;
using MFramework.Core;
using MFramework.Core.Event;
using UnityEngine;

namespace MFrameworkExamples.Event
{
    public class TestEvent : IEvent
    {
        
    }

    public class TestEventArgs : EventArgs
    {
        
    }
    
    public class MEntry : MEntryBase
    {
        protected override void OnBootstrapped(TrackerStoppedEvent e)
        {
            MEventBus eventBus = new MEventBus();
            
            // ---IEvent版---
            eventBus.RegisterSafe<TestEvent>((e) =>
            {
                throw new Exception("ERROR");
            });
            eventBus.RegisterSafe<TestEvent>(Test1);
            eventBus.RegisterSafe<TestEvent>(MEntry.Test2);
            
            eventBus.Publish(new TestEvent());
            // ---IEvent版---
            
            // ---一般版---
            eventBus.RegisterSafe("TestEvent", () => throw new Exception("ERROR"));
            eventBus.Publish("TestEvent");
            // ---一般版---
        }

        private void Test1(TestEvent e)
        {
            throw new Exception("ERROR");
        }
        
        private static void Test2(TestEvent e)
        {
            throw new Exception("ERROR");
        }
    }
}