using System;
using System.Collections.Generic;

using MFramework.Core;

using UnityEngine;

namespace MFramework.Pool
{
    /// <summary>
    /// GameObject 对象池服务。
    /// </summary>
    public class MPoolManager
    {
        public const int DefaultAutoWarmSize = 10;

        private readonly Dictionary<GameObject, ObjectPool<GameObject>> _poolsByPrefab =
            new Dictionary<GameObject, ObjectPool<GameObject>>();

        private readonly Dictionary<ObjectPoolContainer<GameObject>, ObjectPool<GameObject>> _poolsByInstance =
            new Dictionary<ObjectPoolContainer<GameObject>, ObjectPool<GameObject>>();

        private readonly Dictionary<ObjectPool<GameObject>, Transform> _rootsByPool =
            new Dictionary<ObjectPool<GameObject>, Transform>();

        private readonly HashSet<Transform> _ownedRoots = new HashSet<Transform>();
        private readonly HashSet<GameObject> _trackedInstances = new HashSet<GameObject>();

        private readonly Transform _defaultParent;
        private readonly int _autoWarmSize;
        private readonly ILog _log;

        public MPoolManager(Transform defaultParent = null, int autoWarmSize = DefaultAutoWarmSize, ILog log = null)
        {
            if (autoWarmSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(autoWarmSize));
            }

            _defaultParent = defaultParent;
            _autoWarmSize = autoWarmSize;
            _log = log ?? new UserLog(nameof(MPoolManager));
        }

        public void WarmPool(GameObject prefab, Transform parent, int size)
        {
            if (!TryValidatePrefab(prefab))
            {
                return;
            }

            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            if (_poolsByPrefab.ContainsKey(prefab))
            {
                _log.W($"{nameof(MPoolManager)}：{prefab.name} 已经建池，忽略重复 WarmPool。");
                return;
            }

            Transform poolRoot = ResolvePoolRoot(prefab, parent);
            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(() =>
            {
                GameObject instance = UnityEngine.Object.Instantiate(prefab, poolRoot);
                instance.SetActive(false);
                _trackedInstances.Add(instance);
                return instance;
            }, size);

            _poolsByPrefab[prefab] = pool;
            _rootsByPool[pool] = poolRoot;
        }

        public void WarmPool(GameObject prefab, Transform parent)
        {
            WarmPool(prefab, parent, 0);
        }

        public ObjectPoolContainer<GameObject> SpawnObject(GameObject prefab)
        {
            return SpawnObject(prefab, Vector3.zero, Quaternion.identity);
        }

        public ObjectPoolContainer<GameObject> SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!TryValidatePrefab(prefab))
            {
                return null;
            }

            if (!_poolsByPrefab.ContainsKey(prefab))
            {
                WarmPool(prefab, null, _autoWarmSize);
            }

            ObjectPool<GameObject> pool = _poolsByPrefab[prefab];
            ObjectPoolContainer<GameObject> container = pool.GetItem();
            GameObject instance = container.Item;

            instance.transform.SetPositionAndRotation(position, rotation);
            instance.SetActive(true);
            _poolsByInstance[container] = pool;

            return container;
        }

        public bool ReleaseObject(ObjectPoolContainer<GameObject> container)
        {
            if (container == null)
            {
                return false;
            }

            if (!_poolsByInstance.TryGetValue(container, out ObjectPool<GameObject> pool))
            {
                _log.W($"{nameof(MPoolManager)}：对象未登记到池中，无法回收。");
                return false;
            }

            if (container.Item != null)
            {
                container.Item.SetActive(false);
                if (_rootsByPool.TryGetValue(pool, out Transform poolRoot) && poolRoot != null)
                {
                    container.Item.transform.SetParent(poolRoot, false);
                }
            }

            bool released = pool.ReleaseItem(container);
            if (released)
            {
                _poolsByInstance.Remove(container);
            }

            return released;
        }

        public void Shutdown()
        {
            foreach (GameObject instance in _trackedInstances)
            {
                if (instance != null)
                {
                    UnityEngine.Object.Destroy(instance);
                }
            }

            foreach (Transform root in _ownedRoots)
            {
                if (root != null)
                {
                    UnityEngine.Object.Destroy(root.gameObject);
                }
            }

            _trackedInstances.Clear();
            _ownedRoots.Clear();
            _rootsByPool.Clear();
            _poolsByInstance.Clear();
            _poolsByPrefab.Clear();
        }

        private bool TryValidatePrefab(GameObject prefab)
        {
            if (prefab != null)
            {
                return true;
            }

            _log.W($"{nameof(MPoolManager)}：prefab 为空，已忽略本次请求。");
            return false;
        }

        private Transform ResolvePoolRoot(GameObject prefab, Transform parent)
        {
            if (parent != null)
            {
                return parent;
            }

            if (_defaultParent != null)
            {
                return _defaultParent;
            }

            string prefabName = prefab.name;
            if (!string.IsNullOrEmpty(prefabName))
            {
                prefabName = char.ToUpperInvariant(prefabName[0]) + prefabName.Substring(1);
            }
            else
            {
                prefabName = "Pool";
            }

            GameObject group = new GameObject($"{prefabName}Group");
            _ownedRoots.Add(group.transform);
            return group.transform;
        }
    }
}
