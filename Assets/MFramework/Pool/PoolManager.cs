using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        public bool logStatus;
        public Transform root;

        private Dictionary<GameObject, ObjectPool<GameObject>> prefabLookup;
        private Dictionary<GameObject, ObjectPool<GameObject>> instanceLookup;

        private bool dirty = false;

        protected void Awake()
        {
            prefabLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
            instanceLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
        }

        void Update()
        {
            if (logStatus && dirty)
            {
                PrintStatus();
                dirty = false;
            }
        }
        public void warmPool(GameObject prefab, int size, Transform parent = null)
        {
            //prefabLookup中不应该存在prefabKey
            if (prefabLookup.ContainsKey(prefab))
            {
                Log.Print("Pool for prefab " + prefab.name + " has already been created", MLogType.Error);
            }

            //正常情况---初始化对象池，就是调用ObjectPool的构造函数
            var pool = new ObjectPool<GameObject>(() => { return InstantiatePrefab(prefab, parent); }, size);
            prefabLookup[prefab] = pool;

            dirty = true;
        }

        public GameObject spawnObject(GameObject prefab)
        {
            return spawnObject(prefab, Vector3.zero, Quaternion.identity);
        }

        public GameObject spawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!prefab)
                return null;

            //如果在prefabLookup(Project表)中没有prefabKey，说明是第一次，需要先WarmPool()
            if (!prefabLookup.ContainsKey(prefab))
            {
                WarmPool(prefab, 1);
            }

            var pool = prefabLookup[prefab];

            var clone = pool.GetItem();//获取池中可用对象
            if (clone == null)
                return null;

            //设置初始状态
            clone.transform.position = position;
            clone.transform.rotation = rotation;
            clone.SetActive(true);

            instanceLookup.Add(clone, pool);//即clone属于pool
            dirty = true;
            return clone;
        }

        public void releaseObject(GameObject clone)
        {
            if (!clone)
                return;

            //释放的本质就是关闭物体
            clone.SetActive(false);

            if (instanceLookup.ContainsKey(clone))
            {
                //删除表中两个表中的键值对并将Used设为false
                instanceLookup[clone].ReleaseItem(clone);
                instanceLookup.Remove(clone);
                dirty = true;
            }
            else
            {
                Log.Print("No pool contains the object: " + clone.name, MLogType.Error);
            }
        }


        private GameObject InstantiatePrefab(GameObject prefab, Transform parent = null)
        {
            var go = GameObject.Instantiate(prefab, parent) as GameObject;
            if (root != null) go.transform.SetParent(root, true);
            return go;
        }

        public void PrintStatus()
        {
            foreach (KeyValuePair<GameObject, ObjectPool<GameObject>> keyVal in prefabLookup)
            {
                Log.Print(string.Format("Object Pool for Prefab: {0} In Use: {1} Total {2}", keyVal.Key.name,
                    keyVal.Value.CountUsedItems, keyVal.Value.Count), MLogType.Error);
            }
        }

        //静态部分---用于调用，实际调用的都是上侧首字母小写版本
        //将物体预热---生成时会预热
        public static void WarmPool(GameObject prefab, int size, Transform parent = null)
        {
            Instance.warmPool(prefab, size, parent);
        }
        //生成物体至对象池
        public static GameObject SpawnObject(GameObject prefab)
        {
            return Instance.spawnObject(prefab);
        }
        public static GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return Instance.spawnObject(prefab, position, rotation);
        }
        //从对象池移除
        public static void ReleaseObject(GameObject clone)
        {
            Instance.releaseObject(clone);
        }
    }
}