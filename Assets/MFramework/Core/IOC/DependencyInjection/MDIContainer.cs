using System;
using System.Collections.Generic;
using System.Dynamic;
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
        public Type SourceType;
        [CanBeNull] public string Name;

        public Key(Type sourceType, string name)
        {
            SourceType = sourceType;
            Name = name;
        }
    }
    
    public class MDIContainer
    {
        private Dictionary<Key, object> _instances;
        private Dictionary<Key, object> _scopedInstances = new Dictionary<Key, object>();
        private Dictionary<Key, Binding> _bindings;

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
        

        public MDIContainer CreateScope()
        {
            return new MDIContainer(_parent);
        }

        public void Register(Type sourceType, string name, object targetInstance, Lifecycle lifecycle)
        {
            var key = new Key(sourceType, name);
            Bindings[key] = new Binding(lifecycle, (container) => targetInstance);
        }

        public void Register(Type sourceType, string name, Type targetType, Lifecycle lifecycle)
        {
            var key = new Key(sourceType, name);
            Bindings[key] = new Binding(lifecycle, container => container.CreateInstance(targetType));
        }

        public void Register(Type sourceType, string name, Func<MDIContainer, object> factory, Lifecycle lifecycle)
        {
            var key = new Key(sourceType, name);
            Bindings[key] = new Binding(lifecycle, factory);
        }

        // TODO：开个类写
        private object CreateInstance(Type targetType)
        {
            return null;
        }
    }
}