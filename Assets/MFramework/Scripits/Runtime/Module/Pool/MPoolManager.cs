using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// 对象池系统(仅GameObject)
    /// </summary>
    public class MPoolManager : MonoSingleton<MPoolManager>
    {
        //prefabDic---Prefab与所属的对象池
        //instanceDic---实例与所属的对象池
        private Dictionary<GameObject, ObjectPool<GameObject>> prefabDic;//存放种类(GameObject为Prefab)
        private Dictionary<ObjectPoolContainer<GameObject>, ObjectPool<GameObject>> instanceDic;//存放实例(GameObject为场景中的实例)

        private void Awake()
        {
            prefabDic = new Dictionary<GameObject, ObjectPool<GameObject>>();
            instanceDic = new Dictionary<ObjectPoolContainer<GameObject>, ObjectPool<GameObject>>();
        }

        /// <summary>
        /// 预热池子(预创size个元素)
        /// </summary>
        public void WarmPool(GameObject prefab, Transform parent, int size)
        {
            if (prefabDic.ContainsKey(prefab))//prefab已经入池，无需再次Warm()
            {
                MLog.Print($"{typeof(MPoolManager)}：{prefab.name}已创建，请检查", MLogType.Warning);
                return;
            }

            //正常情况---创建对象池，并存入prefabDic中
            var pool = new ObjectPool<GameObject>(() =>
            {
                //创建函数
                var go = Instantiate(prefab, parent);
                go.SetActive(false);//默认关闭
                return go; 
            }, size);
            prefabDic[prefab] = pool;
        }
        /// <summary>
        /// 预热池子(不进行预创)
        /// </summary>
        public void WarmPool(GameObject prefab, Transform parent)
        {
            if (prefabDic.ContainsKey(prefab))//prefab已经入池，无需再次Warm()
            {
                MLog.Print($"{typeof(MPoolManager)}：{prefab.name}已创建，请检查", MLogType.Warning);
                return;
            }

            //正常情况---创建对象池，并存入prefabDic中
            var pool = new ObjectPool<GameObject>(() =>
            {
                //创建函数
                var go = Instantiate(prefab, parent);
                go.SetActive(false);//默认关闭
                return go;
            });
            prefabDic[prefab] = pool;
        }

        /// <summary>
        /// 创建物体
        /// </summary>
        public ObjectPoolContainer<GameObject> SpawnObject(GameObject prefab)
        {
            return SpawnObject(prefab, Vector3.zero, Quaternion.identity);
        }
        /// <summary>
        /// 创建物体
        /// </summary>
        public ObjectPoolContainer<GameObject> SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!prefab) return null;

            //如果在prefabDic中没有prefabKey，说明是第一次，需要先WarmPool()
            if (!prefabDic.ContainsKey(prefab))
            {
                //创建父物体
                string prefabName = prefab.name;
                prefabName = char.ToUpper(prefabName[0]) + prefabName.Substring(1);
                GameObject parent = new GameObject($"{prefabName}Group");

                WarmPool(prefab, parent.transform, 10);//暖池(初始容量为10)
            }

            var pool = prefabDic[prefab];

            var clone = pool.GetItem();//获取池中可用对象
            if (clone == null) return null;

            //设置初始状态(简易初始化)
            GameObject go = clone.Item;
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.SetActive(true);

            instanceDic.Add(clone, pool);
            return clone;
        }

        /// <summary>
        /// 释放物体
        /// </summary>
        public void ReleaseObject(ObjectPoolContainer<GameObject> container)
        {
            if (container == null) return;

            if (instanceDic.ContainsKey(container))
            {
                //清除并将Used设为false
                container.Item.SetActive(false);
                instanceDic[container].ReleaseItem(container);
                instanceDic.Remove(container);
            }
            else
            {
                MLog.Print($"{typeof(MPoolManager)}：{container.Item.name}不存在于池中，请检查", MLogType.Warning);
            }
        }
    }
}