using System;
using System.Collections.Generic;
using MFramework.Core.Internal;

namespace MFramework.Core.Event
{
    public interface IEvent { }

    public class MEventBus
    {
        private readonly Dictionary<string, List<Action>> _eventHandlers = new Dictionary<string, List<Action>>();
        private readonly Dictionary<string, Dictionary<Action, Action>> _safeEventHandlers =
            new Dictionary<string, Dictionary<Action, Action>>();

        private readonly Dictionary<Type, List<Delegate>> _typeEventHandler = new Dictionary<Type, List<Delegate>>();
        private readonly Dictionary<Type, Dictionary<Delegate, Delegate>> _safeTypeEventHandlers =
            new Dictionary<Type, Dictionary<Delegate, Delegate>>();

        public Action<string> LogError { get; set; }

        public MEventBus()
        {

        }

        public void Register(string eventName, Action handler)
        {
            RegisterInternal(eventName, handler);
        }

        public void RegisterSafe(string eventName, Action handler)
        {
            if (handler == null) return;

            if (TryGetSafeHandler(eventName, handler, out Action existingSafeHandler))
            {
                RegisterInternal(eventName, existingSafeHandler);
                return;
            }

            var rLocation = IntUtilEx.GetCallerLocation(2);
            void SafeHandler()
            {
                try
                {
                    handler();
                }
                catch (FrameworkException)
                {
                    throw; // 来自框架的throw直接抛出(因为是致命错误，应该抛出)
                }
                catch (Exception ex)
                {
                    var pLocation = IntUtilEx.GetCallerLocation(3);
                    LogError?.Invoke(GetExceptionLog(eventName, rLocation, pLocation, ex.Message));
                }
            }

            AddSafeHandler(eventName, handler, SafeHandler);
            RegisterInternal(eventName, SafeHandler);
        }

        private void RegisterInternal(string eventName, Action handler)
        {
            if (handler == null) return;

            if (!_eventHandlers.TryGetValue(eventName, out var handlers))
            {
                handlers = new List<Action>();
                _eventHandlers[eventName] = handlers;
            }

            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }

        public void Publish(string eventName)
        {
            if (!_eventHandlers.TryGetValue(eventName, out var handlers))
            {
                return;
            }

            foreach (var handler in handlers.ToArray())
            {
                handler.Invoke();
            }
        }

        public bool Remove(string eventName, Action handler)
        {
            if (handler == null) return false;

            bool removed = RemoveHandler(eventName, handler);

            if (TryRemoveSafeHandler(eventName, handler, out Action safeHandler))
            {
                removed |= RemoveHandler(eventName, safeHandler);
            }

            return removed;
        }

        public void Clear(string eventName)
        {
            _eventHandlers.Remove(eventName);
            _safeEventHandlers.Remove(eventName);
        }

        public void Clear()
        {
            _eventHandlers.Clear();
            _safeEventHandlers.Clear();
            _typeEventHandler.Clear();
            _safeTypeEventHandlers.Clear();
        }



        public void Register<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            RegisterInternal(handler);
        }

        public void RegisterSafe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            if (handler == null) return;

            if (TryGetSafeHandler(handler, out Action<TEvent> existingSafeHandler))
            {
                RegisterInternal(existingSafeHandler);
                return;
            }

            var rLocation = IntUtilEx.GetCallerLocation(2);
            void SafeHandler(TEvent e)
            {
                try
                {
                    handler(e);
                }
                catch (FrameworkException)
                {
                    throw; // 来自框架的throw直接抛出(因为是致命错误，应该抛出)
                }
                catch (Exception ex)
                {
                    var pLocation = IntUtilEx.GetCallerLocation(3);
                    LogError?.Invoke(GetExceptionLog(typeof(TEvent).FullName, rLocation, pLocation, ex.Message));
                }
            }

            AddSafeHandler(handler, SafeHandler);
            RegisterInternal((Action<TEvent>)SafeHandler);
        }

        private void RegisterInternal<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            if (handler == null) return;

            if (!_typeEventHandler.TryGetValue(typeof(TEvent), out var handlers))
            {
                handlers = new List<Delegate>();
                _typeEventHandler[typeof(TEvent)] = handlers;
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

        public bool Remove<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            if (handler == null) return false;

            bool removed = RemoveHandler(handler);

            if (TryRemoveSafeHandler(handler, out Action<TEvent> safeHandler))
            {
                removed |= RemoveHandler(safeHandler);
            }

            return removed;
        }

        public void Clear<TEvent>() where TEvent : IEvent
        {
            Type eventType = typeof(TEvent);
            _typeEventHandler.Remove(eventType);
            _safeTypeEventHandlers.Remove(eventType);
        }

        private bool RemoveHandler(string eventName, Action handler)
        {
            if (!_eventHandlers.TryGetValue(eventName, out var handlers))
            {
                return false;
            }

            int removedCount = handlers.RemoveAll(registeredHandler => registeredHandler == handler);
            if (handlers.Count == 0)
            {
                _eventHandlers.Remove(eventName);
            }

            return removedCount > 0;
        }

        private bool RemoveHandler<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            Type eventType = typeof(TEvent);
            if (!_typeEventHandler.TryGetValue(eventType, out var handlers))
            {
                return false;
            }

            int removedCount = handlers.RemoveAll(registeredHandler => registeredHandler == (Delegate)handler);
            if (handlers.Count == 0)
            {
                _typeEventHandler.Remove(eventType);
            }

            return removedCount > 0;
        }

        private bool TryGetSafeHandler(string eventName, Action handler, out Action safeHandler)
        {
            safeHandler = null;
            return _safeEventHandlers.TryGetValue(eventName, out var handlers) &&
                   handlers.TryGetValue(handler, out safeHandler);
        }

        private void AddSafeHandler(string eventName, Action handler, Action safeHandler)
        {
            if (!_safeEventHandlers.TryGetValue(eventName, out var handlers))
            {
                handlers = new Dictionary<Action, Action>();
                _safeEventHandlers[eventName] = handlers;
            }

            handlers[handler] = safeHandler;
        }

        private bool TryRemoveSafeHandler(string eventName, Action handler, out Action safeHandler)
        {
            safeHandler = null;
            if (!_safeEventHandlers.TryGetValue(eventName, out var handlers) ||
                !handlers.TryGetValue(handler, out safeHandler))
            {
                return false;
            }

            handlers.Remove(handler);
            if (handlers.Count == 0)
            {
                _safeEventHandlers.Remove(eventName);
            }

            return true;
        }

        private bool TryGetSafeHandler<TEvent>(Action<TEvent> handler, out Action<TEvent> safeHandler)
            where TEvent : IEvent
        {
            safeHandler = null;
            Type eventType = typeof(TEvent);
            if (!_safeTypeEventHandlers.TryGetValue(eventType, out var handlers) ||
                !handlers.TryGetValue(handler, out Delegate registeredSafeHandler))
            {
                return false;
            }

            safeHandler = (Action<TEvent>)registeredSafeHandler;
            return true;
        }

        private void AddSafeHandler<TEvent>(Action<TEvent> handler, Action<TEvent> safeHandler)
            where TEvent : IEvent
        {
            Type eventType = typeof(TEvent);
            if (!_safeTypeEventHandlers.TryGetValue(eventType, out var handlers))
            {
                handlers = new Dictionary<Delegate, Delegate>();
                _safeTypeEventHandlers[eventType] = handlers;
            }

            handlers[handler] = safeHandler;
        }

        private bool TryRemoveSafeHandler<TEvent>(Action<TEvent> handler, out Action<TEvent> safeHandler)
            where TEvent : IEvent
        {
            safeHandler = null;
            Type eventType = typeof(TEvent);
            if (!_safeTypeEventHandlers.TryGetValue(eventType, out var handlers) ||
                !handlers.TryGetValue(handler, out Delegate registeredSafeHandler))
            {
                return false;
            }

            handlers.Remove(handler);
            if (handlers.Count == 0)
            {
                _safeTypeEventHandlers.Remove(eventType);
            }

            safeHandler = (Action<TEvent>)registeredSafeHandler;
            return true;
        }

        private string GetExceptionLog(string eventName, CallerLocation rLocation, CallerLocation pLocation, string message)
        {
            // 注意：打包后信息有限，文件与行数信息都会缺失
            return $"触发 {eventName} 事件时发生错误：\n" +
                   $"Register处：{rLocation}    Publish处：{pLocation}\n" +
                   $"{message}";
        }
    }
}
