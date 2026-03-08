using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using MFramework.Core.CoreEx;
using MFramework.Core.Event;

namespace MFramework.Core
{
    public enum Lifecycle
    {
        Transient,
        Scoped,
        Singleton,
    }
    
    public class Binding
    {
        public Lifecycle Lifecycle { get; }

        public Func<MDIContainer, object> Factory { get; }

        public Binding(Lifecycle lifecycle, Func<MDIContainer, object> factory)
        {
            Lifecycle = lifecycle;
            Factory = factory;
        }
    }

    public class Key
    {
        internal Type SourceType { get; }
        
        [CanBeNull] internal string Name { get; }

        public Key(Type sourceType, string name)
        {
            SourceType = sourceType;
            Name = name;
        }
        
        public override bool Equals(object obj)
        {
            return Equals(obj as Key);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(SourceType, Name);
        }

        public override string ToString()
        {
            return $"{SourceType}|{Name ?? "NULL"}";
        }

        private bool Equals(Key other)
        {
            if (other == null) return false;
            
            return SourceType == other.SourceType &&
                   string.Equals(Name, other.Name, StringComparison.Ordinal);
        }
        
        public static bool operator ==(Key left, Key right)
        {
            return EqualityComparer<Key>.Default.Equals(left, right);
        }

        public static bool operator !=(Key left, Key right)
        {
            return !(left == right);
        }
    }
    
    public class MDIContainer : IDisposable
    {
        private static readonly ILog _log = new InternalLog(nameof(MDIContainer));
        
        private Dictionary<Key, object> _instances;
        private Dictionary<Key, object> _scopedInstances = new Dictionary<Key, object>();
        private Dictionary<Key, Binding> _bindings; // 子container没有_bindings

        private List<IDisposable> _disposables = new List<IDisposable>();
        
        private MDIContainer Root => _parent == null ? this : _parent.Root;

        private Dictionary<Key, object> Instances => Root._instances;
        private Dictionary<Key, Binding> Bindings => Root._bindings;
        
        private MDIContainer _parent;

        public MDIContainer()
        {
            _parent = null;
            
            _instances = new Dictionary<Key, object>();
            _bindings = new Dictionary<Key, Binding>();
        }
        
        private MDIContainer(MDIContainer parent)
        {
            _parent = parent;
        }
        
        public void Dispose()
        {
            if (_disposables == null) return;

            for (int i = _disposables.Count - 1; i >= 0; i--)
            {
                _disposables[i].Dispose();
            }

            _disposables.Clear();
        }

        public MDIContainer CreateScope()
        {
            return new MDIContainer(this);
        }

        #region 原型
        
        internal void RegisterInstance(Type sourceType, string name, object targetInstance)
        {
            var key = new Key(sourceType, name);
            ValidKey(key);

            Bindings[key] = new Binding(Lifecycle.Singleton, (_) => targetInstance);
        }

        internal void RegisterType(Type sourceType, string name, Type targetType, Lifecycle lifecycle)
        {
            var key = new Key(sourceType, name);
            ValidKey(key);
            
            Bindings[key] = new Binding(lifecycle, container => container.CreateInstance(targetType, name));
        }

        internal void RegisterFactory(Type sourceType, string name, Func<MDIContainer, object> factory, Lifecycle lifecycle)
        {
            var key = new Key(sourceType, name);
            ValidKey(key);

            Bindings[key] = new Binding(lifecycle, factory);
        }

        public object Resolve(Type sourceType, string name)
        {
            var key = new Key(sourceType, name);

            if (!Bindings.TryGetValue(key, out var binding))
            {
                throw new UnityFrameworkException($"未注册类型: {key}");
            }

            switch (binding.Lifecycle)
            {
                case Lifecycle.Singleton:
                    return ResolveSingleton(key, binding);
                
                case Lifecycle.Scoped:
                    return ResolveScoped(key, binding);
                
                case Lifecycle.Transient:
                    return ResolveTransient(key, binding);
                
                default:
                    throw new UnityFrameworkException($"未知Lifecycle：{binding.Lifecycle}");
            }
        }

        private object ResolveSingleton(Key key, Binding binding)
        {
            if (!Instances.TryGetValue(key, out var instance))
            {
                instance = binding.Factory(this);
                if (instance == null)
                {
                    throw new UnityFrameworkException($"解析失败：{key}");
                }
                TrackDisposable(instance, binding.Lifecycle);
                Instances[key] = instance;
            }
            return instance;
        }

        private object ResolveScoped(Key key, Binding binding)
        {
            if (_parent == null)
            {
                throw new UnityFrameworkException($"根容器下禁止解析Scoped：{key}");
            }
                    
            if (!_scopedInstances.TryGetValue(key, out var instance))
            {
                instance = binding.Factory(this);
                if (instance == null)
                {
                    throw new UnityFrameworkException($"解析失败：{key}");
                }
                _scopedInstances[key] = instance;
            }
            return instance;
        }

        private object ResolveTransient(Key key, Binding binding)
        {
            var instance = binding.Factory(this);
            if (instance == null)
            {
                throw new UnityFrameworkException($"解析失败：{key}");
            }
            return instance;
        }
        
        private object CreateInstance(Type targetType, string name)
        {
            // 递归解析
            ConstructorInfo[] constructors = targetType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (constructors.Length == 0)
            {
                _log.W($"无可用public构造：{targetType}");
                return null;
            }
            
            // 取最多的那个构造函数
            var suitableParameters = constructors.First().GetParameters();
            foreach (var c in constructors)
            {
                var parameters = c.GetParameters();
                if (parameters.Length > suitableParameters.Length)
                {
                    suitableParameters = parameters;
                }
            }

            var args = suitableParameters.Select(p =>
            {
                if (p.ParameterType.IsArray)
                {
                    var elementType = p.ParameterType.GetElementType();
                    return ResolveAll(elementType);
                }
                
                if (p.ParameterType.IsGenericType &&
                    p.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    var elementType = p.ParameterType.GetGenericArguments()[0];
                    return ResolveAll(elementType);
                }
                
                return Resolve(p.ParameterType, name);
            }).ToArray();
            
            var obj = Activator.CreateInstance(targetType, args);
            return obj;
        }

        private void TrackDisposable(object instance, Lifecycle lifecycle)
        {
            if (instance is not IDisposable disposable) return;

            switch (lifecycle)
            {
                case Lifecycle.Singleton:
                {
                    Root._disposables.Add(disposable);
                    break;
                }

                case Lifecycle.Scoped:
                {
                    _disposables.Add(disposable);
                    break;
                }

                case Lifecycle.Transient:
                {
                    break; // Transient不进行管理
                }
                
                default:
                {
                    throw new UnityFrameworkException($"未知Lifecycle：{lifecycle}");
                }
            }
        }

        private void ValidKey(Key key)
        {
            if (Bindings.ContainsKey(key))
            {
                throw new UnityFrameworkException($"重复注册，Key：{key}");
            }
            if (key.SourceType.IsPrimitive || key.SourceType == typeof(string))
            {
                throw new UnityFrameworkException($"基础类型无法注册，Key：{key}");
            }
        }
        
        private Array ResolveAll(Type sourceType)
        {
            var matched = Bindings.Keys
                .Where(k => k.SourceType == sourceType)
                .ToList();

            var array = Array.CreateInstance(sourceType, matched.Count);

            for (int i = 0; i < matched.Count; i++)
            {
                var instance = Resolve(matched[i].SourceType, matched[i].Name);
                array.SetValue(instance, i);
            }

            return array;
        }
        
        #endregion
    }

    public static class MDIContainerExtensions
    {
        # region Instance
        
        // Singleton特有注册方式
        public static void RegisterSingleton<TSource>(this MDIContainer container, string name, object targetInstance)
        {
            container.RegisterInstance(typeof(TSource), name, targetInstance);
        }
        
        // Singleton特有注册方式
        public static void RegisterSingleton<TSource>(this MDIContainer container, object targetInstance)
        {
            container.RegisterInstance(typeof(TSource), null, targetInstance);
        }
        
        public static void RegisterSingleton<TSource>(this MDIContainer container, string name)
        {
            container.RegisterType(typeof(TSource), name, typeof(TSource), Lifecycle.Singleton);
        }
        
        public static void RegisterSingleton<TSource>(this MDIContainer container)
        {
            container.RegisterType(typeof(TSource), null, typeof(TSource), Lifecycle.Singleton);
        }

        public static void RegisterSingleton<TSource, TTarget>(this MDIContainer container, string name)
        {
            container.RegisterType(typeof(TSource), name, typeof(TTarget), Lifecycle.Singleton);
        }
        
        public static void RegisterSingleton<TSource, TTarget>(this MDIContainer container)
        {
            container.RegisterType(typeof(TSource), null, typeof(TTarget), Lifecycle.Singleton);
        }
        
        public static void RegisterSingleton<TSource>(this MDIContainer container, string name, Func<MDIContainer, TSource> factory)
        {
            Func<MDIContainer, object> wrappedFactory = c => factory(c);
            container.RegisterFactory(typeof(TSource), name, wrappedFactory, Lifecycle.Singleton);
        }
        
        public static void RegisterSingleton<TSource>(this MDIContainer container, Func<MDIContainer, TSource> factory)
        {
            Func<MDIContainer, object> wrappedFactory = c => factory(c);
            container.RegisterFactory(typeof(TSource), null, wrappedFactory, Lifecycle.Singleton);
        }

        public static void RegisterSingleton<TSource, TTarget>(this MDIContainer container, string name, Func<MDIContainer, TTarget> factory)
        {
            Func<MDIContainer, object> wrappedFactory = c => factory(c);
            container.RegisterFactory(typeof(TSource), name, wrappedFactory, Lifecycle.Singleton);
        }

        public static void RegisterSingleton<TSource, TTarget>(this MDIContainer container, Func<MDIContainer, TTarget> factory)
        {
            Func<MDIContainer, object> wrappedFactory = c => factory(c);
            container.RegisterFactory(typeof(TSource), null, wrappedFactory, Lifecycle.Singleton);
        }
        
        # endregion
        
        # region Scoped
        
        public static void RegisterScoped<TSource>(this MDIContainer container, string name)
        {
            container.RegisterType(typeof(TSource), name, typeof(TSource), Lifecycle.Scoped);
        }
        
        public static void RegisterScoped<TSource>(this MDIContainer container)
        {
            container.RegisterType(typeof(TSource), null, typeof(TSource), Lifecycle.Scoped);
        }

        public static void RegisterScoped<TSource, TTarget>(this MDIContainer container, string name)
        {
            container.RegisterType(typeof(TSource), name, typeof(TTarget), Lifecycle.Scoped);
        }
        
        public static void RegisterScoped<TSource, TTarget>(this MDIContainer container)
        {
            container.RegisterType(typeof(TSource), null, typeof(TTarget), Lifecycle.Scoped);
        }
        
        public static void RegisterScoped<TSource>(this MDIContainer container, string name, Func<MDIContainer, TSource> factory)
        {
            Func<MDIContainer, object> wrappedFactory = c => factory(c);
            container.RegisterFactory(typeof(TSource), name, wrappedFactory, Lifecycle.Scoped);
        }
        
        public static void RegisterScoped<TSource>(this MDIContainer container, Func<MDIContainer, TSource> factory)
        {
            Func<MDIContainer, object> wrappedFactory = c => factory(c);
            container.RegisterFactory(typeof(TSource), null, wrappedFactory, Lifecycle.Scoped);
        }

        public static void RegisterScoped<TSource, TTarget>(this MDIContainer container, string name, Func<MDIContainer, TTarget> factory)
        {
            Func<MDIContainer, object> wrappedFactory = c => factory(c);
            container.RegisterFactory(typeof(TSource), name, wrappedFactory, Lifecycle.Scoped);
        }

        public static void RegisterScoped<TSource, TTarget>(this MDIContainer container, Func<MDIContainer, TTarget> factory)
        {
            Func<MDIContainer, object> wrappedFactory = c => factory(c);
            container.RegisterFactory(typeof(TSource), null, wrappedFactory, Lifecycle.Scoped);
        }
        
        # endregion
        
        # region Transient
        
        public static void RegisterTransient<TSource>(this MDIContainer container, string name)
        {
            container.RegisterType(typeof(TSource), name, typeof(TSource), Lifecycle.Transient);
        }
        
        public static void RegisterTransient<TSource>(this MDIContainer container)
        {
            container.RegisterType(typeof(TSource), null, typeof(TSource), Lifecycle.Transient);
        }

        public static void RegisterTransient<TSource, TTarget>(this MDIContainer container, string name)
        {
            container.RegisterType(typeof(TSource), name, typeof(TTarget), Lifecycle.Transient);
        }
        
        public static void RegisterTransient<TSource, TTarget>(this MDIContainer container)
        {
            container.RegisterType(typeof(TSource), null, typeof(TTarget), Lifecycle.Transient);
        }
        
        public static void RegisterTransient<TSource>(this MDIContainer container, string name, Func<MDIContainer, TSource> factory)
        {
            Func<MDIContainer, object> wrappedFactory = c => factory(c);
            container.RegisterFactory(typeof(TSource), name, wrappedFactory, Lifecycle.Transient);
        }
        
        public static void RegisterTransient<TSource>(this MDIContainer container, Func<MDIContainer, TSource> factory)
        {
            Func<MDIContainer, object> wrappedFactory = c => factory(c);
            container.RegisterFactory(typeof(TSource), null, wrappedFactory, Lifecycle.Transient);
        }

        public static void RegisterTransient<TSource, TTarget>(this MDIContainer container, string name, Func<MDIContainer, TTarget> factory)
        {
            Func<MDIContainer, object> wrappedFactory = c => factory(c);
            container.RegisterFactory(typeof(TSource), name, wrappedFactory, Lifecycle.Transient);
        }

        public static void RegisterTransient<TSource, TTarget>(this MDIContainer container, Func<MDIContainer, TTarget> factory)
        {
            Func<MDIContainer, object> wrappedFactory = c => factory(c);
            container.RegisterFactory(typeof(TSource), null, wrappedFactory, Lifecycle.Transient);
        }
        
        # endregion

        public static T Resolve<T>(this MDIContainer container, string name)
        {
            return (T)container.Resolve(typeof(T), name);
        }
        
        public static T Resolve<T>(this MDIContainer container)
        {
            return (T)container.Resolve(typeof(T), null);
        }
    }
}