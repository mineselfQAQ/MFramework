using System;
using System.Collections.Generic;

using MFramework.Core;

namespace MFramework.Pool
{
    /// <summary>
    /// 通用对象池，只管理创建、借出与归还逻辑。
    /// </summary>
    public class ObjectPool<T>
    {
        private static readonly ILog _log = new UserLog(nameof(ObjectPool<T>));

        private readonly Stack<ObjectPoolContainer<T>> _unusedContainers;
        private readonly HashSet<ObjectPoolContainer<T>> _allContainers;
        private readonly HashSet<ObjectPoolContainer<T>> _usedContainers;
        private readonly Func<T> _factory;

        public int UnusedCount => _unusedContainers.Count;

        public int UsedCount => _usedContainers.Count;

        public int TotalCount => _allContainers.Count;

        public ObjectPool(Func<T> factory, int initSize)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (initSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initSize));
            }

            _factory = factory;
            _unusedContainers = new Stack<ObjectPoolContainer<T>>(initSize);
            _allContainers = new HashSet<ObjectPoolContainer<T>>();
            _usedContainers = new HashSet<ObjectPoolContainer<T>>();

            Warm(initSize);
        }

        public ObjectPool(Func<T> factory) : this(factory, 0)
        {
        }

        public void Warm(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            for (int i = 0; i < count; i++)
            {
                CreateContainer();
            }
        }

        public ObjectPoolContainer<T> GetItem()
        {
            if (_unusedContainers.Count == 0)
            {
                CreateContainer();
            }

            ObjectPoolContainer<T> container = _unusedContainers.Pop();
            container.Consume();
            _usedContainers.Add(container);
            return container;
        }

        public bool ReleaseItem(ObjectPoolContainer<T> container)
        {
            if (container == null)
            {
                _log.W($"{typeof(ObjectPool<T>)} 归还失败：句柄为空。");
                return false;
            }

            if (!_allContainers.Contains(container))
            {
                _log.W($"{typeof(ObjectPool<T>)} 归还失败：句柄不属于当前对象池。");
                return false;
            }

            if (!_usedContainers.Contains(container) || !container.Used)
            {
                _log.W($"{typeof(ObjectPool<T>)} 归还失败：句柄当前未被使用。");
                return false;
            }

            _usedContainers.Remove(container);
            container.Release();
            _unusedContainers.Push(container);
            return true;
        }

        private ObjectPoolContainer<T> CreateContainer()
        {
            ObjectPoolContainer<T> container = new ObjectPoolContainer<T>(_factory());
            _allContainers.Add(container);
            _unusedContainers.Push(container);
            return container;
        }
    }
}
