using System;
using System.Collections.Generic;

namespace MFramework.DLC
{
    public class MMinHeap<T>
    {
        private MList<T> _heap;

        private IComparer<T> comparer;

        public int Count
        {
            get
            {
                return _heap.Count;
            }
        }

        public MMinHeap()
        {
            _heap = new MList<T>();

            this.comparer = Comparer<T>.Default;
        }
        public MMinHeap(IComparer<T> comparer)
        {
            _heap = new MList<T>();

            this.comparer = comparer;
        }
        public MMinHeap(IEnumerable<T> nums)
        {
            this.comparer = Comparer<T>.Default;

            _heap = new MList<T>(nums);

            int size = GetParent(Count - 1);//获取迭代数量(根节点到最后一个非叶子节点)
            //倒序对每一个非叶子节点进行堆化操作
            for (int i = size; i >= 0; i--)
            {
                SiftDown(i);
            }
        }
        public MMinHeap(IEnumerable<T> nums, IComparer<T> comparer)
        {
            this.comparer = comparer;

            _heap = new MList<T>(nums);

            int size = GetParent(Count - 1);//获取迭代数量(根节点到最后一个非叶子节点)
            //倒序对每一个非叶子节点进行堆化操作
            for (int i = size; i >= 0; i--)
            {
                SiftDown(i);
            }
        }

        public T Peek()
        {
            return _heap[0];
        }

        public void Push(T item)
        {
            _heap.Add(item);//将元素填入堆底
            SiftUp(Count - 1);//自底向上的堆化操作
        }

        public T Pop()
        {
            if (Count == 0) throw new Exception();

            Swap(0, Count - 1);//交换堆顶元素与堆底元素

            //暂存后弹出堆底元素
            T item = _heap[Count - 1];
            _heap.RemoveAt(Count - 1);

            SiftDown(0);//自顶向下堆化操作

            return item;
        }

        private void SiftUp(int i)
        {
            while (true)
            {
                int p = GetParent(i);

                //条件满足，不再执行(父节点索引出界 或 子元素大于等于父元素)
                if (p < 0 || comparer.Compare(_heap[i], _heap[p]) >= 0)
                {
                    break;
                }

                Swap(i, p);//交换节点中的值
                i = p;//向上一层
            }
        }

        private void SiftDown(int i)
        {
            while (true)
            {
                //获取左右节点与当前节点
                int l = GetLeft(i), r = GetRight(i), max = i;

                //子节点未出界 且 父节点并非最大，此时更改max
                if (l < Count && comparer.Compare(_heap[l], _heap[max]) < 0)
                {
                    max = l;
                }
                if (r < Count && comparer.Compare(_heap[r], _heap[max]) < 0)
                {
                    max = r;
                }

                if (max == i)//条件满足，不再执行(父节点为最大值)
                {
                    break;
                }

                Swap(i, max);//交换节点中的值
                i = max;//向下一层
            }
        }

        private void Swap(int i1, int i2)
        {
            T temp = _heap[i1];
            _heap[i1] = _heap[i2];
            _heap[i2] = temp;
        }

        private int GetLeft(int i)
        {
            return 2 * i + 1;
        }
        private int GetRight(int i)
        {
            return 2 * i + 2;
        }
        private int GetParent(int i)
        {
            return (i - 1) / 2;
        }
    }
}