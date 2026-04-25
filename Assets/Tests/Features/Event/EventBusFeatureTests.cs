using MFramework.Core.Event;
using NUnit.Framework;

namespace MFramework.Tests.Event
{
    public class EventBusFeatureTests
    {
        private class TestEvent : IEvent
        {
        }

        private class OtherTestEvent : IEvent
        {
        }

        [Test]
        public void RemoveStringHandler_AfterRegister_StopsReceivingEvent()
        {
            var eventBus = new MEventBus();
            int callCount = 0;
            void Handler() => callCount++;

            eventBus.Register("test", Handler);
            bool removed = eventBus.UnRegister("test", Handler);
            eventBus.Publish("test");

            Assert.That(removed, Is.True);
            Assert.That(callCount, Is.EqualTo(0));
        }

        [Test]
        public void ClearStringEvent_RemovesOnlySelectedEventName()
        {
            var eventBus = new MEventBus();
            int firstCallCount = 0;
            int secondCallCount = 0;

            eventBus.Register("first", () => firstCallCount++);
            eventBus.Register("second", () => secondCallCount++);

            eventBus.Clear("first");
            eventBus.Publish("first");
            eventBus.Publish("second");

            Assert.That(firstCallCount, Is.EqualTo(0));
            Assert.That(secondCallCount, Is.EqualTo(1));
        }

        [Test]
        public void RemoveTypedHandler_AfterRegister_StopsReceivingEvent()
        {
            var eventBus = new MEventBus();
            int callCount = 0;
            void Handler(TestEvent e) => callCount++;

            eventBus.Register<TestEvent>(Handler);
            bool removed = eventBus.UnRegister<TestEvent>(Handler);
            eventBus.Publish(new TestEvent());

            Assert.That(removed, Is.True);
            Assert.That(callCount, Is.EqualTo(0));
        }

        [Test]
        public void ClearTypedEvent_RemovesOnlySelectedEventType()
        {
            var eventBus = new MEventBus();
            int firstCallCount = 0;
            int secondCallCount = 0;

            eventBus.Register<TestEvent>(_ => firstCallCount++);
            eventBus.Register<OtherTestEvent>(_ => secondCallCount++);

            eventBus.Clear<TestEvent>();
            eventBus.Publish(new TestEvent());
            eventBus.Publish(new OtherTestEvent());

            Assert.That(firstCallCount, Is.EqualTo(0));
            Assert.That(secondCallCount, Is.EqualTo(1));
        }

        [Test]
        public void ClearAll_RemovesStringAndTypedHandlers()
        {
            var eventBus = new MEventBus();
            int stringCallCount = 0;
            int typedCallCount = 0;

            eventBus.Register("test", () => stringCallCount++);
            eventBus.Register<TestEvent>(_ => typedCallCount++);

            eventBus.Clear();
            eventBus.Publish("test");
            eventBus.Publish(new TestEvent());

            Assert.That(stringCallCount, Is.EqualTo(0));
            Assert.That(typedCallCount, Is.EqualTo(0));
        }

        [Test]
        public void RemoveSafeStringHandler_WithOriginalHandler_StopsReceivingEvent()
        {
            var eventBus = new MEventBus();
            int callCount = 0;
            void Handler() => callCount++;

            eventBus.RegisterSafe("test", Handler);
            bool removed = eventBus.UnRegister("test", Handler);
            eventBus.Publish("test");

            Assert.That(removed, Is.True);
            Assert.That(callCount, Is.EqualTo(0));
        }

        [Test]
        public void RemoveSafeTypedHandler_WithOriginalHandler_StopsReceivingEvent()
        {
            var eventBus = new MEventBus();
            int callCount = 0;
            void Handler(TestEvent e) => callCount++;

            eventBus.RegisterSafe<TestEvent>(Handler);
            bool removed = eventBus.UnRegister<TestEvent>(Handler);
            eventBus.Publish(new TestEvent());

            Assert.That(removed, Is.True);
            Assert.That(callCount, Is.EqualTo(0));
        }
    }
}
