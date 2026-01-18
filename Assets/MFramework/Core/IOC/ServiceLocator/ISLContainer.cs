using System;

namespace MFramework.Core
{
    public interface ISLContainer : ISLContainerRegistry, ISLContainerProvider
    {
    }

    public interface ISLContainerRegistry
    {
        void RegisterTransient<T>(T instance);
        void RegisterTransient<T>(Func<T> factory);
        void RegisterInstance<T>(T instance);
        void RegisterInstance<T>(Func<T> factory);
    }
    
    public interface ISLContainerProvider
    {
        T Resolve<T>();
    }

    public interface ISLContainerChecker
    {
        bool IsRegistered<T>();
        bool IsTransient<T>();
        bool IsInstance<T>();
    }
}