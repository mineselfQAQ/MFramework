using System;
using System.Collections.Generic;
using MFramework.Core.Internal;

namespace MFramework.Core.Event
{
    public interface IEvent { }

    public class MEventBus
    {
        private class HandlerEntry
        {
            public Action Handler;
            public bool IsSafe;

            public CallerLocation RegisterLocation;
        }

        private class TypeHandlerEntry
        {
            public Delegate Handler;
            public bool IsSafe;

            public CallerLocation RegisterLocation;
        }

        private readonly Dictionary<string, List<HandlerEntry>> _eventHandlers = new();

        private readonly Dictionary<Type, List<TypeHandlerEntry>> _typeEventHandlers = new();

        public Action<string> LogError { get; set; }

        public MEventBus()
        {

        }

        public void Clear()
        {
            _eventHandlers.Clear();
            _typeEventHandlers.Clear();
        }


        # region string版
        public void Register(string eventName, Action handler)
        {
            var rLocation = IntUtilEx.GetCallerLocation(2);
            RegisterInternal(eventName, handler, isSafe: false, rLocation);
        }

        public void RegisterSafe(string eventName, Action handler)
        {
            var rLocation = IntUtilEx.GetCallerLocation(2);
            RegisterInternal(eventName, handler, isSafe: true, rLocation);
        }

        private void RegisterInternal(string eventName, Action handler, bool isSafe, CallerLocation rLocation)
        {
            if (handler == null) return;

            if (!_eventHandlers.TryGetValue(eventName, out var entries))
            {
                entries = new List<HandlerEntry>();
                _eventHandlers[eventName] = entries;
            }

            if (entries.Exists(entry => entry.Handler == handler)) return;
            entries.Add(new HandlerEntry()
            {
                Handler = handler,
                IsSafe = isSafe,

                RegisterLocation = rLocation,
            });
        }

        public void Publish(string eventName)
        {
            if (!_eventHandlers.TryGetValue(eventName, out var entries))
                return;

            foreach (var entry in entries.ToArray())
            {
                if (entry.IsSafe)
                {
                    try
                    {
                        entry.Handler.Invoke();
                    }
                    catch (FrameworkException)
                    {
                        throw; // 来自框架的throw直接抛出(因为是致命错误，应该抛出)
                    }
                    catch (Exception ex)
                    {
                        var pLocation = IntUtilEx.GetCallerLocation(2);
                        LogError?.Invoke(GetExceptionLog(eventName, entry.RegisterLocation, pLocation, ex.Message));
                    }
                }
                else
                {
                    entry.Handler.Invoke();
                }
            }
        }

        public bool UnRegister(string eventName, Action handler)
        {
            if (handler == null) return false;
            if (!_eventHandlers.TryGetValue(eventName, out var entries))
                return false;

            int removed = entries.RemoveAll(entry => entry.Handler == handler);

            if (entries.Count == 0) _eventHandlers.Remove(eventName);

            return removed > 0;
        }

        public void Clear(string eventName)
        {
            _eventHandlers.Remove(eventName);
        }
        #endregion


        # region IEvent版
        public void Register<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            var rLocation = IntUtilEx.GetCallerLocation(2);
            RegisterInternal(handler, isSafe: false, rLocation);
        }

        public void RegisterSafe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            var rLocation = IntUtilEx.GetCallerLocation(2);
            RegisterInternal(handler, isSafe: true, rLocation);
        }

        private void RegisterInternal<TEvent>(Action<TEvent> handler, bool isSafe, CallerLocation rLocation) where TEvent : IEvent
        {
            if (handler == null) return;

            Type eventType = typeof(TEvent);
            if (!_typeEventHandlers.TryGetValue(eventType, out var entries))
            {
                entries = new List<TypeHandlerEntry>();
                _typeEventHandlers[eventType] = entries;
            }

            if (entries.Exists(entry => entry.Handler == (Delegate)handler)) return;
            entries.Add(new TypeHandlerEntry
            {
                Handler = handler,
                IsSafe = isSafe,

                RegisterLocation = rLocation,
            });
        }

        public void Publish<TEvent>(TEvent eventData) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);

            if (_typeEventHandlers.TryGetValue(eventType, out var entries))
            {
                foreach (var entry in entries.ToArray())
                {
                    if (entry.Handler is not Action<TEvent> typedHandler)
                        continue;

                    if (entry.IsSafe)
                    {
                        try
                        {
                            typedHandler.Invoke(eventData);
                        }
                        catch (FrameworkException)
                        {
                            throw; // 来自框架的throw直接抛出(因为是致命错误，应该抛出)
                        }
                        catch (Exception ex)
                        {
                            var pLocation = IntUtilEx.GetCallerLocation(2);
                            LogError?.Invoke(GetExceptionLog(typeof(TEvent).FullName, entry.RegisterLocation, pLocation, ex.Message));
                        }
                    }
                    else
                    {
                        typedHandler.Invoke(eventData);
                    }
                }
            }
        }

        public bool UnRegister<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            if (handler == null) return false;

            Type eventType = typeof(TEvent);

            if (!_typeEventHandlers.TryGetValue(eventType, out var entries))
                return false;

            int removed = entries.RemoveAll(entry => entry.Handler == (Delegate)handler);

            if (entries.Count == 0)
                _typeEventHandlers.Remove(eventType);

            return removed > 0;
        }

        public void Clear<TEvent>() where TEvent : IEvent
        {
            _typeEventHandlers.Remove(typeof(TEvent));
        }
        #endregion

        private string GetExceptionLog(string eventName, CallerLocation rLocation, CallerLocation pLocation, string message)
        {
            // 注意：打包后信息有限，文件与行数信息都会缺失
            return $"触发 {eventName} 事件时发生错误：\n" +
                   $"Register处：{rLocation}    Publish处：{pLocation}\n" +
                   $"{message}";
        }
    }
}
