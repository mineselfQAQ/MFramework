using System;
using System.Collections;
using System.Collections.Generic;

namespace MFramework.DLC
{
    public class MList<T> : IEnumerable<T>
    {
        private const int _deafultCapacity = 4;

        private T[] _items;

        private int _count;

        public int Count
        {
            get
            {
                return _count;
            }
        }

        public int Capacity
        {
            get
            {
                return _items.Length;
            }
            set
            {
                if (value < _count)
                {
                    throw new Exception();
                }

                if (value == _items.Length)
                {
                    return;
                }

                T[] newArray = new T[value];
                if (_count > 0)
                {
                    Array.Copy(_items, 0, newArray, 0, _count);
                }
                _items = newArray;
            }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _count)
                {
                    throw new Exception();
                }

                return _items[index];
            }
            set
            {
                if (index < 0 || index >= _count)
                {
                    throw new Exception();
                }

                _items[index] = value;
            }
        }

        public MList()
        {
            _items = new T[_deafultCapacity];
        }
        public MList(int capacity)
        {
            if (capacity < 0) throw new Exception();

            _items = new T[capacity];
        }
        public MList(IEnumerable<T> nums)
        {
            if (nums == null) throw new Exception();

            _count = 0;
            _items = new T[0];
            foreach (T item in nums)
            {
                Add(item);
            }
        }

        public void Add(T item)
        {
            if (_count == _items.Length)//需要扩容
            {
                EnsureCapacity(_count + 1);
            }

            _items[_count++] = item;
        }

        public void Insert(int index, T item)
        {
            //注意:这里不是index>=_size，因为如果index=_size时，是一种可以直接在尾部添加的形式
            if (index < 0 || index > _count) throw new Exception();

            if (_count == _items.Length)//需要扩容
            {
                EnsureCapacity(_count + 1);
            }

            //一般情况复制(向后挪一位)，如果index=_size，也就是说插入位置为尾元素后一位，那么不用挪
            if (index < _count)
            {
                Array.Copy(_items, index, _items, index + 1, _count - index);
            }

            _items[index] = item;
            _count++;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _count) throw new Exception();

            _count--;//先减
            if (index < _count)//排除index为尾元素情况
            {
                Array.Copy(_items, index + 1, _items, index, _count - index);
            }
            _items[_count] = default(T);
        }

        /// <summary>
        /// 清空元素，保留数组
        /// </summary>
        public void Clear()
        {
            if (_count > 0)
            {
                Array.Clear(_items, 0, _count);
                _count = 0;
            }
        }

        public bool Contains(T item)
        {
            //查找null元素情况
            if (item == null)
            {
                for (int i = 0; i < _count; i++)
                {
                    if (_items[i] == null)
                    {
                        return true;
                    }
                }
                return false;
            }

            //查找一般元素情况
            for (int i = 0; i < _count; i++)
            {
                if (_items[i].Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item, 0, _count);
        }

        public void Reverse()
        {
            Reverse(0, _count);
        }
        public void Reverse(int index, int count)
        {
            //只要确保有值即可
            if (index < 0) throw new Exception();
            if (count < 0) throw new Exception();

            //可操作元素比将要操作元素少，不成立
            if (_count - index < count) throw new Exception();

            Array.Reverse(_items, index, count);
        }

        public void Sort()
        {
            Sort(0, _count, null);
        }
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            //只要确保有值即可
            if (index < 0) throw new Exception();
            if (count < 0) throw new Exception();

            //可操作元素比将要操作元素少，不成立
            if (_count - index < count) throw new Exception();

            Array.Sort(_items, index, count, comparer);
        }

        public void TrimExcess()
        {
            int num = (int)((double)_items.Length * 0.9);//阈值，0.9总容量

            //如果占用率不高(还没有超过阈值)，可以进行裁剪
            //也就是说本来就已经快满了，那么还不如先填满自动扩容
            if (_count < num)
            {
                Capacity = _count;
            }
        }

        /// <summary>
        /// 确保容量大于等于min
        /// </summary>
        /// <param playerName="min"></param>
        private void EnsureCapacity(int min)
        {
            if (_items.Length < min)//确实容量不够了
            {
                int num = ((_items.Length == 0) ? 4 : (_items.Length * 2));

                if ((uint)num > 2146435071u)//限制最大值
                {
                    num = 2146435071;
                }

                if (num < min)//扩容后还是比最小值小
                {
                    num = min;//使用最小值
                }

                Capacity = num;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
            {
                yield return _items[i];
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
            {
                yield return _items[i];
            }
        }
    }

    public static class MListExtension
    {
        public static void Print<T>(this MList<T> list)
        {
            MLog.Print("输出: ");

            if (list.Count == 0)
            {
                MLog.Print("无元素");
                return;
            }

            string outputStr = "";
            foreach (T item in list)
            {
                outputStr += $"{item} ";
            }
            MLog.Print(outputStr);
        }
    }
}