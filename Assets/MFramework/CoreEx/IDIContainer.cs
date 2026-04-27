using System;

namespace MFramework.Core.CoreEx
{
    public interface IDIContainer : IDisposable
    {
        void RegisterSingleton<TSource>(object targetInstance);

        void RegisterSingleton<TSource>(Func<IDIContainer, TSource> factory);

        T Resolve<T>();

        bool UnRegister<TSource>();
    }
}
