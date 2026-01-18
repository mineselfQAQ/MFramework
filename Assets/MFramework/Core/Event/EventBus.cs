using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework.Core.Event
{
    public interface IEvent { }
    
    public class EventBus
    {
        private readonly ILog _log = new InternalLog("EventBus");
        
        private readonly Dictionary<string, List<Delegate>> _eventHandlers =  new Dictionary<string, List<Delegate>>();
        
        private readonly Dictionary<Type, List<Delegate>> _typeEventHandler = new Dictionary<Type, List<Delegate>>();
        
        public EventBus()
        {
            
        }
        
        public void Register(string eventName, EventHandler handler)
        {
            RegisterInternal(eventName, handler);
        }
        
        public void Register<TEventArgs>(string eventName, EventHandler<TEventArgs> handler) 
            where TEventArgs : EventArgs
        {
            RegisterInternal(eventName, handler);
        }

        public void Register(string eventName, Action handler)
        {
            RegisterInternal(eventName, handler);
        }
        
        public void Register<TEvent>(string eventName, Action<TEvent> handler)
        {
            RegisterInternal(eventName, handler);
        }
        
        private void RegisterInternal(string eventName, Delegate handler)
        {
            if (!_eventHandlers.TryGetValue(eventName, out var handlers))
            {
                handlers = new List<Delegate>();
                _eventHandlers[eventName] = handlers;
            }

            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }
        
        public void Publish(string eventName, object sender, EventArgs args)
        {
            PublishInternal(eventName, sender, args);
        }
        
        public void Publish<TEventArgs>(string eventName, object sender, TEventArgs args)
            where TEventArgs : EventArgs
        {
            PublishInternal(eventName, sender, args);
        }

        public void Publish<TEvent>(string eventName, TEvent eventData)
        {
            PublishInternal(eventName, null, eventData);
        }

        public void Publish(string eventName)
        {
            PublishInternal(eventName, null, null);
        }

        private void PublishInternal(string eventName, object sender, object args)
        {
            if (!_eventHandlers.TryGetValue(eventName, out var handlers))
            {
                return;
            }
            
            foreach (var handler in handlers)
            {
                switch (handler)
                {
                    case EventHandler<EventArgs> eh when args is EventArgs eventArgs:
                        eh.Invoke(sender, eventArgs);
                        break;
                    case Action action when args == null:
                        action.Invoke();
                        break;
                    case Delegate del:
                        del.DynamicInvoke(sender, args);
                        break;
                }
            }
        }
        
        public void Register<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
        
            if (!_typeEventHandler.TryGetValue(eventType, out var handlers))
            {
                handlers = new List<Delegate>();
                _typeEventHandler[eventType] = handlers;
            }

            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }

        public void Publish<TEvent>(TEvent eventData) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
        
            if (_typeEventHandler.TryGetValue(eventType, out var handlers))
            {
                foreach (var handler in handlers.ToArray())
                {
                    if (handler is Action<TEvent> typedHandler)
                    {
                        typedHandler.Invoke(eventData);
                    }
                }
            }
        }
    }
}