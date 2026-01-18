using System;
using System.Collections.Generic;

namespace MFramework.Core
{
    /// <summary>
    /// IOC的Service Locator实现，仅实现最基础功能
    /// </summary>
    public class MSLContainer : ISLContainer, ISLContainerChecker
    {
        private readonly Dictionary<Type, object> _factoryDic = new Dictionary<Type, object>(); // Key-Type Value-Entry<T>
        
        public void RegisterTransient<T>(T instance)
        {
            Register<T>(new TransientFactory<T>(() => instance));
        }
        
        public void RegisterTransient<T>(Func<T> factory)
        {
            Register<T>(new TransientFactory<T>(factory));
        }
        
        public void RegisterInstance<T>(T instance)
        {
            Register<T>(new InstanceFactory<T>(() => instance));
        }
        
        public void RegisterInstance<T>(Func<T> factory)
        {
            Register<T>(new InstanceFactory<T>(factory));
        }

        public T Resolve<T>()
        {
            if (_factoryDic.TryGetValue(typeof(T), out object entryObj))
            {
                Entry<T> entry = (Entry<T>)entryObj;
                return entry.Factory.Create();
            }
            return default;
        }

        public bool IsRegistered<T>()
        {
            return _factoryDic.ContainsKey(typeof(T));
        }

        public bool IsTransient<T>()
        {
            if (_factoryDic.TryGetValue(typeof(T), out object entryObj))
            {
                Entry<T> entry = (Entry<T>)entryObj;
                if(entry.Mode == SLMode.Transient)
                {
                    return true;
                }
            }
            return false;
        }
        
        public bool IsInstance<T>()
        {
            if (_factoryDic.TryGetValue(typeof(T), out object entryObj))
            {
                Entry<T> entry = (Entry<T>)entryObj;
                if(entry.Mode == SLMode.Instance)
                {
                    return true;
                }
            }
            return false;
        }

        private void Register<T>(IFactory<T> factory)
        {
            if (factory == null) return;
            
            if (_factoryDic.ContainsKey(typeof(T)))
            {
                // 错误：已注册
            }

            if (factory is IInstanceFactory<T>)
            {
                _factoryDic[typeof(T)] = new Entry<T>(factory, SLMode.Instance);
                
            }
            else if (factory is ITransientFactory<T>)
            {
                _factoryDic[typeof(T)] = new Entry<T>(factory, SLMode.Transient);
            }
        }

        private enum SLMode
        {
            Instance, // 单例
            Transient, // 瞬态
        }
        
        private class Entry<T> : IDisposable
        {
            internal IFactory<T> Factory { get; }
            internal SLMode Mode { get; }
            
            internal Entry(IFactory<T> factory, SLMode mode)
            {
                Factory = factory;
                Mode = mode;
            }

            public void Dispose()
            {
                Factory.Dispose();
            }
        }
    
        private class InstanceFactory<T> : IInstanceFactory<T>
        {
            private readonly Lazy<T> _instance;
        
            public InstanceFactory(Func<T> factory)
            {
                _instance = new Lazy<T>(factory);
            }
        
            object IFactory.Create()
            {
                return Create();
            }
            
            public T Create()
            {
                return _instance.Value;
            }
        
            public void Dispose()
            {
                if (_instance.IsValueCreated && _instance.Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        private class TransientFactory<T> : ITransientFactory<T>
        {
            private readonly Func<T> _factory;

            public TransientFactory(Func<T> factory)
            {
                _factory = factory;
            }

            object IFactory.Create()
            {
                return Create();
            }
            
            public T Create()
            {
                return _factory();
            }

            public void Dispose()
            {
            }
        }

        private interface IFactory : IDisposable
        {
            object Create();
        }
        
        private interface IFactory<T> : IFactory
        {
            new T Create();
        }

        private interface ITransientFactory<T> : IFactory<T> { }
        private interface IInstanceFactory<T> : IFactory<T> { }
    }
}