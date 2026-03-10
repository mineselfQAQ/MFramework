using System;
using System.Reflection;
using System.Threading;
using MFramework.Core.Event;

namespace MFramework.Core.CoreEx
{
    // TODO：AI创建
    public abstract class MSingleton<T> where T : class
    {
        private static readonly Lazy<T> _lazyInstance =
            new Lazy<T>(CreateInstance, LazyThreadSafetyMode.ExecutionAndPublication);

        public static T Instance => _lazyInstance.Value;

        private static T CreateInstance()
        {
            Type targetType = typeof(T);
            ConstructorInfo constructor = targetType.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                Type.EmptyTypes,
                null);

            if (constructor == null)
            {
                throw new CSharpFrameworkException(
                    $"{targetType.FullName} 必须提供无参构造函数（建议 private/protected）。");
            }

            object instance = constructor.Invoke(null);
            if (instance is T typedInstance)
            {
                return typedInstance;
            }

            throw new CSharpFrameworkException($"{targetType.FullName} 单例创建失败。");
        }
    }
}
