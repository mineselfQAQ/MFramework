using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    //�ѷ���---Stack���ʺ϶����
    /// <summary>
    /// �����ϵͳ(��GameObject)
    /// </summary>
    public class Backup_MPoolManager2 : MonoSingleton<Backup_MPoolManager2>
    {
        //prefabDic---Prefab�������Ķ����
        //instanceDic---ʵ���������Ķ����
        private Dictionary<GameObject, Backup_ObjectPool2<GameObject>> prefabDic;//�������(GameObjectΪPrefab)
        private Dictionary<GameObject, Backup_ObjectPool2<GameObject>> instanceDic;//���ʵ��(GameObjectΪ�����е�ʵ��)

        private void Awake()
        {
            prefabDic = new Dictionary<GameObject, Backup_ObjectPool2<GameObject>>();
            instanceDic = new Dictionary<GameObject, Backup_ObjectPool2<GameObject>>();
        }

        /// <summary>
        /// Ԥ�ȳ���
        /// </summary>
        public void WarmPool(GameObject prefab, int size, Transform parent, bool warmObject)
        {
            if (prefabDic.ContainsKey(prefab))//prefab�Ѿ���أ������ٴ�Warm()
            {
                MLog.Print($"{typeof(Backup_MPoolManager2)}��{prefab.name}�Ѵ���������", MLogType.Warning);
                return;
            }

            //�������---��������أ�������prefabDic��
            var pool = new Backup_ObjectPool2<GameObject>(() => { return InstantiatePrefab(prefab, parent); }, size, warmObject);
            prefabDic[prefab] = pool;
        }

        /// <summary>
        /// ��������
        /// </summary>
        public GameObject SpawnObject(GameObject prefab)
        {
            return SpawnObject(prefab, Vector3.zero, Quaternion.identity);
        }
        /// <summary>
        /// ��������
        /// </summary>
        public GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!prefab) return null;

            //�����prefabDic��û��prefabKey��˵���ǵ�һ�Σ���Ҫ��WarmPool()
            if (!prefabDic.ContainsKey(prefab))
            {
                //����������
                string prefabName = prefab.name;
                prefabName = char.ToUpper(prefabName[0]) + prefabName.Substring(1);
                GameObject parent = new GameObject($"{prefabName}Group");

                WarmPool(prefab, 0, parent.transform, false);
            }

            var pool = prefabDic[prefab];

            var clone = pool.GetItem();//��ȡ���п��ö���
            if (clone == null) return null;

            //���ó�ʼ״̬(���׳�ʼ��)
            clone.transform.position = position;
            clone.transform.rotation = rotation;
            clone.SetActive(true);

            instanceDic.Add(clone, pool);
            return clone;
        }

        /// <summary>
        /// �ͷ�����
        /// </summary>
        /// <param name="isInstance">True������ʵ��  False������Prefab</param>
        public GameObject ReleaseObject(GameObject item, bool isInstance)
        {
            if (!item) return null;

            GameObject go = null;
            if (isInstance)//ɾ��ָ��ʵ��
            {
                if (instanceDic.ContainsKey(item))
                {
                    //�������Used��Ϊfalse
                    item.SetActive(false);
                    instanceDic[item].ReleaseItem(item);
                    instanceDic.Remove(item);
                    go = item;
                }
                else
                {
                    MLog.Print($"{typeof(Backup_MPoolManager2)}��{item.name}�������ڳ��У�����", MLogType.Warning);
                }
            }
            else//����ɾ��ʵ��
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
                    MLog.Print($"{typeof(Backup_MPoolManager2)}��{item.name}δ�����أ�����", MLogType.Warning);
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