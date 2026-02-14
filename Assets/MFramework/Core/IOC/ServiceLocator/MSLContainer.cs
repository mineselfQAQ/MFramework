using System;
using System.Collections.Generic;

namespace MFramework.Core
{
    /// <summary>
    /// IOC的Service Locator实现，仅实现最基础功能
    /// </summary>
    public class MSLContainer : ISLContainer, ISLContainerChecker
    {
        private enum SLMode
        {
            Singleton, // 单例
            Transient, // 瞬态
        }
        
        private static ILog _log = new InternalLog(nameof(MSLContainer));
        
        private readonly Dictionary<Type, object> _factoryDic = new Dictionary<Type, object>(); // Key-Type Value-IFactory<T>
        
        public void RegisterTransient<T>(T instance)
        {
            Register<T>(new Factory<T>(() => instance, SLMode.Transient));
        }
        
        public void RegisterTransient<T>(Func<T> factory)
        {
            Register<T>(new Factory<T>(factory, SLMode.Transient));
        }
        
        public void RegisterSingleton<T>(T instance)
        {
            Register<T>(new Factory<T>(() => instance, SLMode.Singleton));
        }
        
        public void RegisterSingleton<T>(Func<T> factory)
        {
            Register<T>(new Factory<T>(factory, SLMode.Singleton));
        }

        public T Resolve<T>()
        {
            if (_factoryDic.TryGetValue(typeof(T), out object obj))
            {
                IFactory<T> factory = (IFactory<T>)obj;
                return factory.Create();
            }
            return default;
        }

        public bool IsRegistered<T>()
        {
            return _factoryDic.ContainsKey(typeof(T));
        }

        public bool IsTransient<T>()
        {
            if (_factoryDic.TryGetValue(typeof(T), out object obj))
            {
                IFactory<T> factory = (IFactory<T>)obj;
                if(factory.Mode == SLMode.Transient)
                {
                    return true;
                }
            }
            return false;
        }
        
        public bool IsInstance<T>()
        {
            if (_factoryDic.TryGetValue(typeof(T), out object obj))
            {
                IFactory<T> factory = (IFactory<T>)obj;
                if(factory.Mode == SLMode.Singleton)
                {
                    return true;
                }
            }
            return false;
        }

        private void Register<T>(IFactory<T> factory)
        {
            if (factory == null)
            {
                _log.EX(LogException.NullArgument);
                return;
            }
            
            if (_factoryDic.ContainsKey(typeof(T)))
            {
                _log.W("已注册");
                return;
            }

            _factoryDic[typeof(T)] = factory;
        }

        private class Factory<T> : IFactory<T>
        {
            private readonly Func<T> _creator;
            private readonly Lazy<T> _lazy;

            public SLMode Mode { get; }

            public Factory(Func<T> creator, SLMode mode)
            {
                Mode = mode;

                if (mode == SLMode.Singleton) _lazy = new Lazy<T>(creator);
                else _creator = creator;
            }

            public T Create()
            {
                return Mode == SLMode.Singleton
                    ? _lazy.Value
                    : _creator();
            }

            object IFactory.Create() => Create();

            public void Dispose()
            {
                if (Mode == SLMode.Singleton &&
                    _lazy.IsValueCreated &&
                    _lazy.Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        private interface IFactory : IDisposable
        {
            object Create();
            SLMode Mode { get; }
        }
        
        private interface IFactory<T> : IFactory
        {
            new T Create();
        }
    }
}