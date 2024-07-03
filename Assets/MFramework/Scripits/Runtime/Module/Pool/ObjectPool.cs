using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// ����أ����д����һ��ObjectPoolContainer<T>
    /// </summary>
    public class ObjectPool<T>
    {
        private List<ObjectPoolContainer<T>> list;
        //ע�⣺ֻ������Used��������ڱ���
        private Dictionary<T, ObjectPoolContainer<T>> lookup;//key---ʵ�ʴ������  value---list�е�һ��Container

        private Func<T> initFunc;
        private int lastIndex = 0;

        public int Count
        {
            get { return list.Count; }
        }
        public int UsedCount
        {
            get { return lookup.Count; }
        }

        public ObjectPool(Func<T> initFunc, int initSize, bool warmObject)
        {
            this.initFunc = initFunc;//ͨ�����캯����ó�ʼ��

            //������ʼlist/lookup
            list = new List<ObjectPoolContainer<T>>(initSize);
            lookup = new Dictionary<T, ObjectPoolContainer<T>>(initSize);
            //������ʼContainer
            if(warmObject) Warm(initSize);
        }

        /// <summary>
        /// ��ȡItem(��ȡNot Used����򴴽�Container)
        /// </summary>
        public T GetItem()
        {
            //��list��Ѱ��Not Used������
            ObjectPoolContainer<T> container = null;
            for (int i = 0; i < list.Count; i++)
            {
                lastIndex++;
                if (lastIndex > list.Count - 1) lastIndex = 0;

                if (list[lastIndex].Used)
                {
                    continue;
                }
                else//�ҵ�Not Used������
                {
                    container = list[lastIndex];
                    break;
                }
            }

            //û���ҵ��������µ�
            if (container == null)
            {
                container = CreateContainer();
            }

            container.Consume();
            lookup.Add(container.Item, container);

            return container.Item;
        }

        /// <summary>
        /// �ͷ�Item(��������)
        /// </summary>
        public void ReleaseItem(object item)
        {
            ReleaseItem((T)item);
        }

        /// <summary>
        /// �ͷ�Item(��������)
        /// </summary>
        public void ReleaseItem(T item)
        {
            if (lookup.ContainsKey(item))
            {
                var container = lookup[item];
                container.Release();
                lookup.Remove(item);
            }
            else//ֻ���ڱ��е�������ǿɱ��ͷ�����
            {
                MLog.Print($"{typeof(ObjectPool<T>)}����û�п��ͷ�{item}������", MLogType.Warning);
            }
        }

        /// <summary>
        /// ����Container
        /// </summary>
        private void Warm(int capacity)
        {
            for (int i = 0; i < capacity; i++)
            {
                var container = CreateContainer();
                (container.Item as GameObject).SetActive(false);
            }
        }

        /// <summary>
        /// ��Container�������
        /// </summary>
        private ObjectPoolContainer<T> CreateContainer()
        {
            //Container�Ĵ�������ʵ�������岢������ӽ�list
            var container = new ObjectPoolContainer<T>();
            container.Item = initFunc();//��ʵ����ִ��InstantiatePrefab()
            list.Add(container);
            return container;
        }
    }
}