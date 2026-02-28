using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

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
        public Lifecycle Lifecycle;

        public Func<MDIContainer, object> Factory;

        public Binding(Lifecycle lifecycle, Func<MDIContainer, object> factory)
        {
            Lifecycle = lifecycle;
            Factory = factory;
        }
    }

    public class Key
    {
        private readonly Type _sourceType;
        [CanBeNull] private readonly string _name;

        public Key(Type sourceType, string name)
        {
            _sourceType = sourceType;
            _name = name;
        }
        
        public override bool Equals(object obj)
        {
            return Equals(obj as Key);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(_sourceType, _name);
        }

        public override string ToString()
        {
            return $"{_sourceType}|{_name}";
        }

        private bool Equals(Key other)
        {
            if (other == null) return false;
            
            return _sourceType == other._sourceType &&
                   string.Equals(_name, other._name, StringComparison.Ordinal);
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
            if(!ValidKey(key)) return;

            Bindings[key] = new Binding(Lifecycle.Singleton, (_) => targetInstance);
        }

        internal void RegisterType(Type sourceType, string name, Type targetType, Lifecycle lifecycle)
        {
            var key = new Key(sourceType, name);
            if(!ValidKey(key)) return;
            
            Bindings[key] = new Binding(lifecycle, container => container.CreateInstance(targetType));
        }

        internal void RegisterFactory(Type sourceType, string name, Func<MDIContainer, object> factory, Lifecycle lifecycle)
        {
            var key = new Key(sourceType, name);
            if(!ValidKey(key)) return;

            Bindings[key] = new Binding(lifecycle, factory);
        }

        public object Resolve(Type sourceType, string name)
        {
            var key = new Key(sourceType, name);

            if (!Bindings.TryGetValue(key, out var binding))
            {
                _log.W($"未注册类型: {key}");
                return null;
            }

            switch (binding.Lifecycle)
            {
                case Lifecycle.Singleton:
                {
                    if (!Instances.TryGetValue(key, out var instance))
                    {
                        instance = binding.Factory(this);
                        TrackDisposable(instance, binding.Lifecycle);
                        Instances[key] = instance;
                    }
                    return instance;
                }
                
                case Lifecycle.Scoped:
                {
                    if (_parent == null)
                    {
                        _log.E("根容器下禁止解析Scoped");
                        throw new Exception("错误"); // TODO：用这个测试Exception报错流程好了(看看要不要写到E()里)
                        return null;
                    }
                    
                    if (!_scopedInstances.TryGetValue(key, out var instance))
                    {
                        instance = binding.Factory(this);
                        _scopedInstances[key] = instance;
                    }
                    return instance;
                }
                
                case Lifecycle.Transient:
                {
                    return binding.Factory(this);
                }
                
                default:
                {
                    _log.E($"未知Lifecycle：{binding.Lifecycle}");
                    return null;
                }
            }
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
                    _disposables.Add(disposable);
                    break;
                }
                
                default:
                {
                    _log.E($"未知Lifecycle：{lifecycle}");
                    break;
                }
            }
        }

        private bool ValidKey(Key key)
        {
            if (Bindings.ContainsKey(key))
            {
                _log.W($"重复注册，Key：{key}");
                return false;
            }

            return true;
        }
        
        // TODO：开个类写
        private object CreateInstance(Type targetType)
        {
            // 递归解析
            ConstructorInfo[] constructors = targetType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (constructors.Length == 0)
            {
                _log.W($"无可用public构造：{targetType}");
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
            
            return null;
        }
        
        #endregion
    }

    public static class MDIContainerExtensions
    {
        # region Instance
        
        public static void RegisterSingleton<TSource>(this MDIContainer container, string name, object targetInstance)
        {
            container.RegisterInstance(typeof(TSource), name, targetInstance);
        }
        
        public static void RegisterSingleton<TSource>(this MDIContainer container, object targetInstance)
        {
            container.RegisterInstance(typeof(TSource), null, targetInstance);
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