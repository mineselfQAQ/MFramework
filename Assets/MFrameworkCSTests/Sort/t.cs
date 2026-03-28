using System.Collections.Generic;

using MFrameworkCSTests.Sort;

namespace MFrameworkCSTests.Sort11
{
    public class Sort1<T>
    {
        public void Sort(IList<T> list, IComparer<T> comparer)
        {
            for (int i = list.Count / 2 - 1; i >= 0; i--)
            {
                SiftDown(list, comparer, i, list.Count - 1);
            }

            int n = 0;
            for (int i = list.Count - 1; i > 0; i--)
            {
                SortUtils.Swap(list, 0, list.Count - 1 - n);
                SiftDown(list, comparer, 0, i);
                n++;
            }
        }

        public void SiftDown(IList<T> list, IComparer<T> comparer, int i, int n)
        {
            while (i < n)
            {
                int left = i * 2 + 1;
                if (left >= n) return;
                int right = i * 2 + 2;

                int index;
                if (right < n && comparer.Compare(list[right], list[left]) > 0)
                {
                    index = right;
                }
                else
                {
                    index = left;
                }

                if (comparer.Compare(list[i], list[index]) > 0) return;

                SortUtils.Swap(list, i, index);
                i = index;
            }
        }
    }
}
