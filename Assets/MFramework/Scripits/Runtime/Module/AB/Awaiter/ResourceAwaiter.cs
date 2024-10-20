﻿using System;
using System.Runtime.CompilerServices;

namespace MFramework
{
    public class ResourceAwaiter : IAwaiter<IResource>, IAwaitable<ResourceAwaiter, IResource>
    {
        public bool IsCompleted { get; private set; }
        public IResource result { get; private set; }

        private Action continuation;

        public IResource GetResult()
        {
            return result;
        }

        public ResourceAwaiter GetAwaiter()
        {
            return this;
        }

        public void OnCompleted(Action continuation)
        {
            if (IsCompleted)
            {
                continuation.Invoke();
            }
            else
            {
                this.continuation += continuation;
            }
        }

        internal void SetResult(IResource result)
        {
            IsCompleted = true;
            this.result = result;
            Action tempCallback = continuation;
            continuation = null;
            tempCallback.Invoke();
        }
    }

    /// <summary>
    /// 用于给 await 确定异步返回的时机，并获取到返回值。
    /// </summary>
    /// <typeparam playerName="TResult">异步返回的返回值类型。</typeparam>
    public interface IAwaiter<out TResult> : INotifyCompletion
    {
        /// <summary>
        /// 获取一个状态，该状态表示正在异步等待的操作已经完成（成功完成或发生了异常）；此状态会被编译器自动调用。
        /// 在实现中，为了达到各种效果，可以灵活应用其值：可以始终为 true，或者始终为 false。
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// 获取此异步等待操作的返回值，此方法会被编译器在 await 结束时自动调用以获取返回值（包括异常）。
        /// </summary>
        /// <returns>异步操作的返回值。</returns>
        TResult GetResult();
    }
    /// <summary>
    /// 表示一个包含返回值的可等待对象，如果一个方法返回此类型的实例，则此方法可以使用 `await` 异步等待返回值。
    /// </summary>
    /// <typeparam playerName="TAwaiter">用于给 await 确定返回时机的 IAwaiter{<typeparamref playerName="TResult"/>} 的实例。</typeparam>
    /// <typeparam playerName="TResult">异步返回的返回值类型。</typeparam>
    public interface IAwaitable<out TAwaiter, out TResult> where TAwaiter : IAwaiter<TResult>
    {
        /// <summary>
        /// 获取一个可用于 await 关键字异步等待的异步等待对象。
        /// 此方法会被编译器自动调用。
        /// </summary>
        TAwaiter GetAwaiter();
    }
}