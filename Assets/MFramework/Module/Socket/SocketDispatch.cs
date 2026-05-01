using System;
using System.Threading;
using MFramework.Core;

namespace MFramework.Socket
{
    public interface ISocketMainThreadDispatcher
    {
        void Capture();
        void Post(Action action);
        void Post<T>(Action<T> action, T arg);
        void Post<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2);
    }

    public sealed class SocketMainThreadDispatcher : ISocketMainThreadDispatcher
    {
        private SynchronizationContext _context;

        public void Capture()
        {
            _context = SynchronizationContext.Current;
        }

        public void Post(Action action)
        {
            if (action == null) return;
            PostInternal(_ => action(), null);
        }

        public void Post<T>(Action<T> action, T arg)
        {
            if (action == null) return;
            PostInternal(state => action((T)state), arg);
        }

        public void Post<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (action == null) return;
            PostInternal(state =>
            {
                var tuple = ((T1, T2))state;
                action(tuple.Item1, tuple.Item2);
            }, (arg1, arg2));
        }

        private void PostInternal(SendOrPostCallback callback, object state)
        {
            SynchronizationContext context = _context;
            if (context == null)
            {
                MLog.Default.W($"{nameof(SocketMainThreadDispatcher)} has no captured SynchronizationContext; invoking callback on current thread.");
                callback(state);
                return;
            }

            context.Post(callback, state);
        }
    }

    internal static class SocketTime
    {
        public static long NowMilliseconds()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}
