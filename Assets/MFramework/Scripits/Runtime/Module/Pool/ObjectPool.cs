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
        //为什么使用Stack：
        //Stack先进后出，访问永远访问栈顶元素(数组最后)，O(1)是最快的
        //而且当物体被使用后，到底何时归还与该类无关，是由业务类自行管理的
        private Stack<ObjectPoolContainer<T>> unusedObj;

        private Func<T> initFunc;

        public int UnusedCount
        {
            get { return unusedObj.Count; }
        }

        public ObjectPool(Func<T> initFunc, int initSize)
        {
            this.initFunc = initFunc;//通过构造函数获得初始化

            //创建初始list/lookup
            unusedObj = new Stack<ObjectPoolContainer<T>>(initSize);
            //创建初始Container
            Warm(initSize);
        }
        public ObjectPool(Func<T> initFunc)
        {
            this.initFunc = initFunc;//通过构造函数获得初始化

            //创建初始list/lookup
            unusedObj = new Stack<ObjectPoolContainer<T>>();
        }

        /// <summary>
        /// 获取Item(获取Not Used物体或创建Container)
        /// </summary>
        public ObjectPoolContainer<T> GetItem()
        {
            ObjectPoolContainer<T> container = null;

            if (unusedObj.Count == 0)//没有Not Used物体
            {
                container = CreateContainer();
            }

            container = unusedObj.Pop();//出队
            container.Consume();

            return container;
        }

        /// <summary>
        /// 释放Item(禁用物体)
        /// </summary>
        public void ReleaseItem(ObjectPoolContainer<T> container)
        {
            if (container.Used)
            {
                unusedObj.Push(container);
                container.Release();
            }
            else//无正在使用物体
            {
                MLog.Print($"{typeof(ObjectPool<T>)}：{container.Item}未使用不可归池，请检查", MLogType.Warning);
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

            unusedObj.Push(container);

            return container;
        }
    }
}