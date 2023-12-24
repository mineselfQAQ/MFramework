using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    public class ObjectPool<T>
    {
        private List<ObjectPoolContainer<T>> list;
        private Dictionary<T, ObjectPoolContainer<T>> lookup;//为instanceLookup
        private Func<T> factoryFunc;
        private int lastIndex = 0;

        public ObjectPool(Func<T> factoryFunc, int initialSize)
        {
            this.factoryFunc = factoryFunc;

            //创建初始list/lookup
            list = new List<ObjectPoolContainer<T>>(initialSize);
            lookup = new Dictionary<T, ObjectPoolContainer<T>>(initialSize);
            //创建初始Container
            Warm(initialSize);
        }

        private void Warm(int capacity)
        {
            for (int i = 0; i < capacity; i++)
            {
                CreateContainer();
            }
        }

        private ObjectPoolContainer<T> CreateContainer()
        {
            //Container的创建就是实例化物体并将其添加进list
            var container = new ObjectPoolContainer<T>();
            container.Item = factoryFunc();//其实就是设置为构造函数中的factoryFunc
            list.Add(container);
            return container;
        }

        public T GetItem()
        {
            //在list中寻找!Used的物体
            ObjectPoolContainer<T> container = null;
            for (int i = 0; i < list.Count; i++)
            {
                lastIndex++;
                if (lastIndex > list.Count - 1) lastIndex = 0;

                if (list[lastIndex].Used)
                {
                    continue;
                }
                else
                {
                    container = list[lastIndex];
                    break;
                }
            }

            if (container == null)
            {
                container = CreateContainer();
            }

            container.Consume();
            lookup.Add(container.Item, container);
            return container.Item;
        }

        public void ReleaseItem(object item)
        {
            ReleaseItem((T)item);
        }

        public void ReleaseItem(T item)
        {
            if (lookup.ContainsKey(item))
            {
                var container = lookup[item];
                container.Release();
                lookup.Remove(item);
            }
            else
            {
                Log.Print("This object pool does not contain the item provided: " + item, MLogType.Error);
            }
        }

        public int Count
        {
            get { return list.Count; }
        }

        public int CountUsedItems
        {
            get { return lookup.Count; }
        }
    }
}