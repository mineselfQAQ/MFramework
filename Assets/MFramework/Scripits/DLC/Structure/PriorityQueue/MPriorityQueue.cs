using System;
using System.Collections.Generic;

namespace MFramework.DLC
{
    /// <summary>
    /// ���ȶ��У�ʹ�ö���ʵ��(Dequeueʱ�临�Ӷ�O(n))
    /// </summary>
    public class MPriorityQueue<TElement, TPriority>
    {
        private List<Tuple<TElement, TPriority>> elements = new List<Tuple<TElement, TPriority>>();

        public int Count
        {
            get { return elements.Count; }
        }

        public void Enqueue(TElement item, TPriority priority)
        {
            //���ȶ��У���ʵ�Ǵ�С�ѣ��������ν
            elements.Add(Tuple.Create(item, priority));
        }

        public TElement Dequeue()
        {
            //���ȶ��У����ѵ�ʱ����Ҫ�ҵ�Priority�����Ǹ�
            Comparer<TPriority> comparer = Comparer<TPriority>.Default;
            int bestIndex = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                if (comparer.Compare(elements[i].Item2, elements[bestIndex].Item2) < 0)
                {
                    bestIndex = i;
                }
            }

            TElement bestItem = elements[bestIndex].Item1;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }
}
