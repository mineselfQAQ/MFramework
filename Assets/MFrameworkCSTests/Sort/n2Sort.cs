using System.Collections.Generic;

namespace MFrameworkCSTests.Sort
{
    /// <summary>
    /// 选择排序
    /// <para>
    /// 选一个最小的，放到未排序区间最前面（通过交换）
    /// 这使得其变为不稳定排序
    /// O(n2)中综合最差的一种
    /// </para>
    /// </summary>
    public class SelectionSort<T>
    {
        public void Sort(IList<T> list, IComparer<T> comparer = null, int way = 0)
        {
            switch (way)
            {
                case 0:
                    Sort1(list, comparer);
                    break;
                case 1:
                    Sort2(list, comparer);
                    break;
            }
        }

        public void Sort1(IList<T> list, IComparer<T> comparer = null)
        {
            comparer ??= Comparer<T>.Default;

            for (int i = 0; i < list.Count; i++)
            {
                // 优化：减少列表访问
                int minIndex = i;
                T minValue = list[i];
                for (int j = i + 1; j < list.Count; j++)
                {
                    T value = list[j];
                    if (comparer.Compare(value, minValue) < 0)
                    {
                        minIndex = j;
                        minValue = value;
                    }
                }

                if (minIndex != i) // 优化：减少无意义Swap
                {
                    SortUtils.Swap(list, i, minIndex);
                }
            }
        }

        public void Sort2(IList<T> list, IComparer<T> comparer = null)
        {
            comparer ??= Comparer<T>.Default;

            int left = 0;
            int right = list.Count - 1;
            // 优化：一次找最小和最大，省一半
            while (left < right)
            {
                int minIndex = left;
                int maxIndex = right;

                T minValue = list[minIndex];
                T maxValue = list[maxIndex];

                for (int i = left; i <= right; i++)
                {
                    T value = list[i];

                    if (comparer.Compare(value, minValue) < 0)
                    {
                        minValue = value;
                        minIndex = i;
                    }

                    if (comparer.Compare(value, maxValue) > 0)
                    {
                        maxValue = value;
                        maxIndex = i;
                    }
                }

                if (minIndex != left)
                {
                    SortUtils.Swap(list, left, minIndex);
                }

                // 修正maxIndex（也许最大值在最左侧，可能被换走）
                // [20, 15, 10]：最大值20被换走了[10, 15, 20]，此时maxIndex的值会跑到minIndex处（因为交换了）
                if (maxIndex == left)
                {
                    maxIndex = minIndex;
                }

                if (maxIndex != right)
                {
                    SortUtils.Swap(list, right, maxIndex);
                }

                left++;
                right--;
            }
        }
    }

    /// <summary>
    /// 冒泡排序
    /// <para>从前往后两两交换，将最大的冒泡到最后
    /// 具有一定的自适应性，越有序交换的越少（依旧需要全比较，除非已完全有序）</para>
    /// </summary>
    public class BubbleSort<T>
    {
        public void Sort(IList<T> list, IComparer<T> comparer = null, int way = 0)
        {

            switch (way)
            {
                case 0:
                    Sort1(list, comparer);
                    break;
                case 1:
                    Sort2(list, comparer);
                    break;
                case 2:
                    Sort3(list, comparer);
                    break;
            }
        }

        public void Sort1(IList<T> list, IComparer<T> comparer = null)
        {
            comparer ??= Comparer<T>.Default;

            for (int i = 0; i < list.Count; i++)
            {
                bool allSort = true;
                for (int j = 0; j < list.Count - i - 1; j++)
                {
                    if (comparer.Compare(list[j], list[j + 1]) > 0)
                    {
                        SortUtils.Swap(list, j, j + 1);
                        allSort = false;
                    }
                }

                if (allSort) break; // 优化：提前退出
            }
        }

        public void Sort2(IList<T> list, IComparer<T> comparer = null)
        {
            comparer ??= Comparer<T>.Default;

            // 优化：不再是每轮冒泡1个，而是根据终止位置决定
            // 即更强的提前退出
            int last = list.Count - 1;
            while (last > 0)
            {
                int newLast = 0;
                for (int i = 0; i < last; i++)
                {
                    if (comparer.Compare(list[i], list[i + 1]) > 0)
                    {
                        SortUtils.Swap(list, i, i + 1);
                        newLast = i;
                    }
                }
                last = newLast;
            }
        }

        public void Sort3(IList<T> list, IComparer<T> comparer = null)
        {
            comparer ??= Comparer<T>.Default;

            int left = 0, right = list.Count - 1;
            while (left < right)
            {
                int newRight = left;

                for (int i = left; i < right; i++)
                {
                    if (comparer.Compare(list[i], list[i + 1]) > 0)
                    {
                        SortUtils.Swap(list, i, i + 1);
                        newRight = i;
                    }
                }

                if (newRight == left) break;
                right = newRight;

                int newLeft = right;

                for (int i = right; i > left; i--)
                {
                    if (comparer.Compare(list[i - 1], list[i]) > 0)
                    {
                        SortUtils.Swap(list, i - 1, i);
                        newLeft = i;
                    }
                }

                if (newLeft == right) break;
                left = newLeft;
            }
        }
    }

    /// <summary>
    /// 插入排序
    /// <para>
    /// 选取未排序区间第一个，向前交换到应在位置，逐步排序
    /// 自适应性极强，每轮仅需考虑最新的一个，前面都是有序的，越有序交换的次数越少
    /// </para>
    /// </summary>
    public class InsertionSort<T> : ISort<T>
    {
        public void Sort(IList<T> list, IComparer<T> comparer = null)
        {
            comparer ??= Comparer<T>.Default;

            for (int i = 1; i < list.Count; i++)
            {
                T current = list[i];
                int j = i - 1;

                // 优化：提前退出：不交换情况少了1次赋值，交换情况多了1次Compare
                // 总的来说意义不大，在有序情况会更赚一点（但Compare更耗）
                // if (comparer.Compare(list[j], current) <= 0) continue;

                // 优化：用shift替代swap
                while (j >= 0 && comparer.Compare(current, list[j]) < 0)
                {
                    list[j + 1] = list[j];
                    j--;
                }

                list[j + 1] = current;
            }
        }
    }
}
