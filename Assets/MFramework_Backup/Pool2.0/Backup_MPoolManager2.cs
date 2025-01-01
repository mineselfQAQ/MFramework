using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    //已废弃---Stack更适合对象池
    /// <summary>
    /// 对象池系统(仅GameObject)
    /// </summary>
    public class Backup_MPoolManager2 : MonoSingleton<Backup_MPoolManager2>
    {
        //prefabDic---Prefab与所属的对象池
        //instanceDic---实例与所属的对象池
        private Dictionary<GameObject, Backup_ObjectPool2<GameObject>> prefabDic;//存放种类(GameObject为Prefab)
        private Dictionary<GameObject, Backup_ObjectPool2<GameObject>> instanceDic;//存放实例(GameObject为场景中的实例)

        private void Awake()
        {
            prefabDic = new Dictionary<GameObject, Backup_ObjectPool2<GameObject>>();
            instanceDic = new Dictionary<GameObject, Backup_ObjectPool2<GameObject>>();
        }

        /// <summary>
        /// 预热池子
        /// </summary>
        public void WarmPool(GameObject prefab, int size, Transform parent, bool warmObject)
        {
            if (prefabDic.ContainsKey(prefab))//prefab已经入池，无需再次Warm()
            {
                MLog.Print($"{typeof(Backup_MPoolManager2)}：{prefab.name}已创建，请检查", MLogType.Warning);
                return;
            }

            //正常情况---创建对象池，并存入prefabDic中
            var pool = new Backup_ObjectPool2<GameObject>(() => { return InstantiatePrefab(prefab, parent); }, size, warmObject);
            prefabDic[prefab] = pool;
        }

        /// <summary>
        /// 创建物体
        /// </summary>
        public GameObject SpawnObject(GameObject prefab)
        {
            return SpawnObject(prefab, Vector3.zero, Quaternion.identity);
        }
        /// <summary>
        /// 创建物体
        /// </summary>
        public GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
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

            //设置初始状态(简易初始化)
            clone.transform.position = position;
            clone.transform.rotation = rotation;
            clone.SetActive(true);

            instanceDic.Add(clone, pool);
            return clone;
        }

        /// <summary>
        /// 释放物体
        /// </summary>
        /// <param name="isInstance">True：传入实例  False：传入Prefab</param>
        public GameObject ReleaseObject(GameObject item, bool isInstance)
        {
            if (!item) return null;

            GameObject go = null;
            if (isInstance)//删除指定实例
            {
                if (instanceDic.ContainsKey(item))
                {
                    //清除并将Used设为false
                    item.SetActive(false);
                    instanceDic[item].ReleaseItem(item);
                    instanceDic.Remove(item);
                    go = item;
                }
                else
                {
                    MLog.Print($"{typeof(Backup_MPoolManager2)}：{item.name}不存在于池中，请检查", MLogType.Warning);
                }
            }
            else//任意删除实例
            {
                if (prefabDic.ContainsKey(item))
                {
                    var temp = prefabDic[item].ReleaseItem();
                    if (temp != null) 
                    {
                        temp.SetActive(false);
                        instanceDic.Remove(temp);
                        go = temp;
                    }
                }
                else
                {
                    MLog.Print($"{typeof(Backup_MPoolManager2)}：{item.name}未创建池，请检查", MLogType.Warning);
                }
            }
            return go;
        }

        private GameObject InstantiatePrefab(GameObject prefab, Transform parent = null)
        {
            return Instantiate(prefab, parent);
        }
    }
}