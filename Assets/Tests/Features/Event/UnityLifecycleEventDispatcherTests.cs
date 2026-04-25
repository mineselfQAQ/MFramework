using System.Reflection;
using MFramework.Core.Event;
using MFramework.Event;
using NUnit.Framework;
using UnityEngine;

namespace MFramework.Tests.Event
{
    public class UnityLifecycleEventDispatcherTests
    {
        [Test]
        public void Update_WhenDispatcherInitialized_PublishesTypedLifecycleEvent()
        {
            var eventBus = new MEventBus();
            var gameObject = new GameObject("UnityLifecycleEventDispatcherTests");
            var dispatcher = gameObject.AddComponent<UnityLifecycleEventDispatcher>();
            int callCount = 0;

            eventBus.Register<UnityUpdateEvent>(e =>
            {
                callCount++;
                Assert.That(e.Dispatcher, Is.EqualTo(dispatcher));
            });

            dispatcher.Initialize(eventBus);
            InvokeUnityMessage(dispatcher, "Update");

            Object.DestroyImmediate(gameObject);
            Assert.That(callCount, Is.EqualTo(1));
        }

        [Test]
        public void OnApplicationFocus_WhenDispatcherInitialized_PublishesFocusState()
        {
            var eventBus = new MEventBus();
            var gameObject = new GameObject("UnityLifecycleEventDispatcherTests");
            var dispatcher = gameObject.AddComponent<UnityLifecycleEventDispatcher>();
            bool? hasFocus = null;

            eventBus.Register<UnityApplicationFocusEvent>(e => hasFocus = e.HasFocus);

            dispatcher.Initialize(eventBus);
            InvokeUnityMessage(dispatcher, "OnApplicationFocus", true);

            Object.DestroyImmediate(gameObject);
            Assert.That(hasFocus, Is.True);
        }

        private static void InvokeUnityMessage(MonoBehaviour target, string methodName, params object[] parameters)
        {
            MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(target, parameters);
        }
    }
}
