using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// ����أ����д����һ��ObjectPoolContainer<T>
    /// </summary>
    public class ObjectPool<T>
    {
        //Ϊʲôʹ��Stack��
        //Stack�Ƚ������������Զ����ջ��Ԫ��(�������)��O(1)������
        //���ҵ����屻ʹ�ú󣬵��׺�ʱ�黹������޹أ�����ҵ�������й����
        private Stack<ObjectPoolContainer<T>> unusedObj;

        private Func<T> initFunc;

        public int UnusedCount
        {
            get { return unusedObj.Count; }
        }

        public ObjectPool(Func<T> initFunc, int initSize)
        {
            this.initFunc = initFunc;//ͨ�����캯����ó�ʼ��

            //������ʼlist/lookup
            unusedObj = new Stack<ObjectPoolContainer<T>>(initSize);
            //������ʼContainer
            Warm(initSize);
        }
        public ObjectPool(Func<T> initFunc)
        {
            this.initFunc = initFunc;//ͨ�����캯����ó�ʼ��

            //������ʼlist/lookup
            unusedObj = new Stack<ObjectPoolContainer<T>>();
        }

        /// <summary>
        /// ��ȡItem(��ȡNot Used����򴴽�Container)
        /// </summary>
        public ObjectPoolContainer<T> GetItem()
        {
            ObjectPoolContainer<T> container = null;

            if (unusedObj.Count == 0)//û��Not Used����
            {
                container = CreateContainer();
            }

            container = unusedObj.Pop();//����
            container.Consume();

            return container;
        }

        /// <summary>
        /// �ͷ�Item(��������)
        /// </summary>
        public void ReleaseItem(ObjectPoolContainer<T> container)
        {
            if (container.Used)
            {
                unusedObj.Push(container);
                container.Release();
            }
            else//������ʹ������
            {
                MLog.Print($"{typeof(ObjectPool<T>)}��{container.Item}δʹ�ò��ɹ�أ�����", MLogType.Warning);
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
            }
        }

        /// <summary>
        /// ��Container�������
        /// </summary>
        private ObjectPoolContainer<T> CreateContainer()
        {
            //Container�Ĵ�������ʵ�������岢������ӽ�Queue
            var container = new ObjectPoolContainer<T>();
            container.Item = initFunc();//��ʵ����ִ��InstantiatePrefab()

            unusedObj.Push(container);

            return container;
        }
    }
}