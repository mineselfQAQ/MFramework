using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// �����ϵͳ(��GameObject)
    /// </summary>
    public class MPoolManager : MonoSingleton<MPoolManager>
    {
        //prefabDic---Prefab�������Ķ����
        //instanceDic---ʵ���������Ķ����
        private Dictionary<GameObject, ObjectPool<GameObject>> prefabDic;//�������(GameObjectΪPrefab)
        private Dictionary<ObjectPoolContainer<GameObject>, ObjectPool<GameObject>> instanceDic;//���ʵ��(GameObjectΪ�����е�ʵ��)

        private void Awake()
        {
            prefabDic = new Dictionary<GameObject, ObjectPool<GameObject>>();
            instanceDic = new Dictionary<ObjectPoolContainer<GameObject>, ObjectPool<GameObject>>();
        }

        /// <summary>
        /// Ԥ�ȳ���(Ԥ��size��Ԫ��)
        /// </summary>
        public void WarmPool(GameObject prefab, Transform parent, int size)
        {
            if (prefabDic.ContainsKey(prefab))//prefab�Ѿ���أ������ٴ�Warm()
            {
                MLog.Print($"{typeof(MPoolManager)}��{prefab.name}�Ѵ���������", MLogType.Warning);
                return;
            }

            //�������---��������أ�������prefabDic��
            var pool = new ObjectPool<GameObject>(() =>
            {
                //��������
                var go = Instantiate(prefab, parent);
                go.SetActive(false);//Ĭ�Ϲر�
                return go; 
            }, size);
            prefabDic[prefab] = pool;
        }
        /// <summary>
        /// Ԥ�ȳ���(������Ԥ��)
        /// </summary>
        public void WarmPool(GameObject prefab, Transform parent)
        {
            if (prefabDic.ContainsKey(prefab))//prefab�Ѿ���أ������ٴ�Warm()
            {
                MLog.Print($"{typeof(MPoolManager)}��{prefab.name}�Ѵ���������", MLogType.Warning);
                return;
            }

            //�������---��������أ�������prefabDic��
            var pool = new ObjectPool<GameObject>(() =>
            {
                //��������
                var go = Instantiate(prefab, parent);
                go.SetActive(false);//Ĭ�Ϲر�
                return go;
            });
            prefabDic[prefab] = pool;
        }

        /// <summary>
        /// ��������
        /// </summary>
        public ObjectPoolContainer<GameObject> SpawnObject(GameObject prefab)
        {
            return SpawnObject(prefab, Vector3.zero, Quaternion.identity);
        }
        /// <summary>
        /// ��������
        /// </summary>
        public ObjectPoolContainer<GameObject> SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!prefab) return null;

            //�����prefabDic��û��prefabKey��˵���ǵ�һ�Σ���Ҫ��WarmPool()
            if (!prefabDic.ContainsKey(prefab))
            {
                //����������
                string prefabName = prefab.name;
                prefabName = char.ToUpper(prefabName[0]) + prefabName.Substring(1);
                GameObject parent = new GameObject($"{prefabName}Group");

                WarmPool(prefab, parent.transform, 10);//ů��(��ʼ����Ϊ10)
            }

            var pool = prefabDic[prefab];

            var clone = pool.GetItem();//��ȡ���п��ö���
            if (clone == null) return null;

            //���ó�ʼ״̬(���׳�ʼ��)
            GameObject go = clone.Item;
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.SetActive(true);

            instanceDic.Add(clone, pool);
            return clone;
        }

        /// <summary>
        /// �ͷ�����
        /// </summary>
        public void ReleaseObject(ObjectPoolContainer<GameObject> container)
        {
            if (container == null) return;

            if (instanceDic.ContainsKey(container))
            {
                //�������Used��Ϊfalse
                container.Item.SetActive(false);
                instanceDic[container].ReleaseItem(container);
                instanceDic.Remove(container);
            }
            else
            {
                MLog.Print($"{typeof(MPoolManager)}��{container.Item.name}�������ڳ��У�����", MLogType.Warning);
            }
        }
    }
}