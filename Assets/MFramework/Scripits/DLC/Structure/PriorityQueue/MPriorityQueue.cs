using System;
using System.Collections.Generic;

namespace MFramework.DLC
{
    /// <summary>
    /// 优先队列，使用队列实现(Dequeue时间复杂度O(n))
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
            //优先队列，其实是大小堆，入堆无所谓
            elements.Add(Tuple.Create(item, priority));
        }

        public TElement Dequeue()
        {
            //优先队列，出堆的时候需要找到Priority最大的那个
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
