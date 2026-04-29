using System;
using System.Collections.Generic;

using MFrameworkCSTests.Sort;

using UnityEditor;

namespace MFrameworkCSTests.Sort11
{
    public class Sort1<T>
    {
        public void Sort(IList<T> list, IComparer<T> comparer)
        {
            Sort(list, comparer, 0, list.Count - 1);
        }

        private void Sort(IList<T> list, IComparer<T> comparer, int left, int right)
        {
            while (left < right)
            {
                int pivot = Partition(list, comparer, left, right);
                if (pivot - left > right - pivot)
                {
                    right = pivot - 1;
                    Sort(list, comparer, pivot + 1, right);
                }
                else
                {
                    left = pivot + 1;
                    Sort(list, comparer, left, pivot - 1);
                }
            }
        }

        private int Partition(IList<T> list, IComparer<T> comparer, int left, int right)
        {
            int mid = left + (right - left) / 2;
            MidThree(list, comparer, left, mid, right);
            SortUtils.Swap(list, left, mid);

            T temp = list[left];
            int i = left, j = right;
            while (i < j)
            {
                while (i < j && comparer.Compare(list[j], temp) <= 0) j--;
                if (i < j)
                {
                    temp = list[j];
                    i++;
                }

                while (i < j && comparer.Compare(list[i], temp) >= 0) i++;
                if (i < j)
                {
                    temp = list[i];
                    j--;
                    i--;
                }
            }

            return 0;
        }

        private void MidThree(IList<T> list, IComparer<T> comparer, int left, int mid, int right)
        {
            if (comparer.Compare(list[left], list[mid]) > 0)
                SortUtils.Swap(list, left, mid);
            if (comparer.Compare(list[left], list[right]) > 0)
                SortUtils.Swap(list, left, right);
            if (comparer.Compare(list[mid], list[right]) > 0)
                SortUtils.Swap(list, mid, right);
        }
    }
}
