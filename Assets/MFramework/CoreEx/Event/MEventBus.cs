using System;
using System.Collections.Generic;
using MFramework.Core.Internal;

namespace MFramework.Core.Event
{
    public interface IEvent { }

    public class MEventBus
    {
        private readonly Dictionary<string, List<Action>> _eventHandlers = new Dictionary<string, List<Action>>();

        private readonly Dictionary<Type, List<Delegate>> _typeEventHandler = new Dictionary<Type, List<Delegate>>();

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

            foreach (var handler in handlers)
            {
                handler.Invoke();
            }
        }



        public void Register<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            RegisterInternal(handler);
        }

        public void RegisterSafe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
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

        private string GetExceptionLog(string eventName, CallerLocation rLocation, CallerLocation pLocation, string message)
        {
            // 注意：打包后信息有限，文件与行数信息都会缺失
            return $"触发 {eventName} 事件时发生错误：\n" +
                   $"Register处：{rLocation}    Publish处：{pLocation}\n" +
                   $"{message}";
        }
    }
}
