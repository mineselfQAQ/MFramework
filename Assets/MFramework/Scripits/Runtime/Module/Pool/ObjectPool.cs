using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// 对象池，其中存放着一组ObjectPoolContainer<T>
    /// </summary>
    public class ObjectPool<T>
    {
        private Queue<ObjectPoolContainer<T>> unusedQueue;
        private Dictionary<T, ObjectPoolContainer<T>> usedLookup;

        private Func<T> initFunc;

        public int Count
        {
            get { return unusedQueue.Count + usedLookup.Count; }
        }
        public int UsedCount
        {
            get { return usedLookup.Count; }
        }

        public ObjectPool(Func<T> initFunc, int initSize, bool warmObject)
        {
            this.initFunc = initFunc;//通过构造函数获得初始化

            //创建初始list/lookup
            unusedQueue = new Queue<ObjectPoolContainer<T>>(initSize);
            usedLookup = new Dictionary<T, ObjectPoolContainer<T>>(initSize);
            //创建初始Container
            if (warmObject) Warm(initSize);
        }

        /// <summary>
        /// 获取Item(获取Not Used物体或创建Container)
        /// </summary>
        public T GetItem()
        {
            ObjectPoolContainer<T> container = null;

            if (unusedQueue.Count == 0)//没有Not Used物体
            {
                container = CreateContainer();
            }

            container = unusedQueue.Dequeue();//出队
            container.Consume();
            usedLookup.Add(container.Item, container);//入表

            return container.Item;
        }

        /// <summary>
        /// 释放Item(禁用物体)
        /// </summary>
        public T ReleaseItem()
        {
            ObjectPoolContainer<T> container = null;

            T temp = default(T);
            if (usedLookup.Count > 0)
            {
                var k = usedLookup.Keys.First();
                container = usedLookup[k];
                temp = container.Item;//取出
                container.Release();
                usedLookup.Remove(k);
                unusedQueue.Enqueue(container);
            }
            else//无正在使用物体
            {
                MLog.Print($"{typeof(ObjectPool<T>)}：已没有可释放物体，请检查", MLogType.Warning);
            }
            return temp;
        }
        /// <summary>
        /// 释放Item(禁用物体)
        /// </summary>
        public void ReleaseItem(T item)
        {
            ObjectPoolContainer<T> container = null;

            if (usedLookup.ContainsKey(item))
            {
                container = usedLookup[item];
                container.Release();
                usedLookup.Remove(item);
                unusedQueue.Enqueue(container);
            }
            else//无正在使用物体
            {
                MLog.Print($"{typeof(ObjectPool<T>)}：已没有可释放{container.Item}，请检查", MLogType.Warning);
            }
        }

        /// <summary>
        /// 创建Container
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
        /// 将Container加入池中
        /// </summary>
        private ObjectPoolContainer<T> CreateContainer()
        {
            //Container的创建就是实例化物体并将其添加进Queue
            var container = new ObjectPoolContainer<T>();
            container.Item = initFunc();//其实就是执行InstantiatePrefab()

            unusedQueue.Enqueue(container);

            return container;
        }
    }
}