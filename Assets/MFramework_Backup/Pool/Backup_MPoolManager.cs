using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    //已废弃---Stack更适合对象池
    public class Backup_MPoolManager : MonoSingleton<Backup_MPoolManager>
    {
        //prefabDic---Prefab与所属的对象池
        //instanceDic---实例与所属的对象池
        private Dictionary<GameObject, Backup_ObjectPool<GameObject>> prefabDic;//存放种类(GameObject为Prefab)
        private Dictionary<GameObject, Backup_ObjectPool<GameObject>> instanceDic;//存放实例(GameObject为场景中的实例)

        private void Awake()
        {
            prefabDic = new Dictionary<GameObject, Backup_ObjectPool<GameObject>>();
            instanceDic = new Dictionary<GameObject, Backup_ObjectPool<GameObject>>();
        }

        /// <summary>
        /// 预热池子
        /// </summary>
        public static void WarmPool(GameObject prefab, int size, Transform parent = null, bool warmObject = true)
        {
            Instance.WarmPoolInternal(prefab, size, parent, warmObject);
        }

        /// <summary>
        /// 创建物体
        /// </summary>
        public static GameObject SpawnObject(GameObject prefab)
        {
            return Instance.SpawnObjectInternal(prefab);
        }
        /// <summary>
        /// 创建物体
        /// </summary>
        public static GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return Instance.SpawnObjectInternal(prefab, position, rotation);
        }

        /// <summary>
        /// 释放物体
        /// </summary>
        public static void ReleaseObject(GameObject clone)
        {
            Instance.ReleaseObjectInternal(clone);
        }

        private void WarmPoolInternal(GameObject prefab, int size, Transform parent, bool warmObject)
        {
            if (prefabDic.ContainsKey(prefab))//prefab已经入池，无需再次Warm()
            {
                MLog.Print($"{typeof(Backup_MPoolManager)}：{prefab.name}已创建，请检查", MLogType.Warning);
                return;
            }

            //正常情况---创建对象池，并存入prefabDic中
            var pool = new Backup_ObjectPool<GameObject>(() => { return InstantiatePrefab(prefab, parent); }, size, warmObject);
            prefabDic[prefab] = pool;
        }

        private GameObject SpawnObjectInternal(GameObject prefab)
        {
            return SpawnObjectInternal(prefab, Vector3.zero, Quaternion.identity);
        }
        private GameObject SpawnObjectInternal(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!prefab) return null;

            //如果在prefabDic中没有prefabKey，说明是第一次，需要先WarmPool()
            if (!prefabDic.ContainsKey(prefab))
            {
                //创建父物体
                string prefabName = prefab.name;
                prefabName = char.ToUpper(prefabName[0]) + prefabName.Substring(1);
                GameObject parent = new GameObject($"{prefabName}Group");

                WarmPool(prefab, 0, parent.transform, false);
            }

            var pool = prefabDic[prefab];

            var clone = pool.GetItem();//获取池中可用对象
            if (clone == null) return null;

            //设置初始状态
            clone.transform.position = position;
            clone.transform.rotation = rotation;
            clone.SetActive(true);

            instanceDic.Add(clone, pool);
            return clone;
        }

        private void ReleaseObjectInternal(GameObject clone)
        {
            if (!clone) return;

            if (instanceDic.ContainsKey(clone))
            {
                //删除表中两个表中的键值对并将Used设为false
                clone.SetActive(false);
                instanceDic[clone].ReleaseItem(clone);
                instanceDic.Remove(clone);
            }
            else
            {
                MLog.Print($"{typeof(Backup_MPoolManager)}：{clone.name}不存在于池中，请检查", MLogType.Warning);
            }
        }

        private GameObject InstantiatePrefab(GameObject prefab, Transform parent = null)
        {
            return Instantiate(prefab, parent);
        }
    }
}