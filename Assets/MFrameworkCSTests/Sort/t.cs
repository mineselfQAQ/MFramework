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
                int pivot = Partition(list, comparer, left, left + (right - left) / 2, right);

                if (pivot - left < right - pivot)
                {
                    Sort(list, comparer, left, pivot - 1);
                    left = pivot + 1;
                }
                else
                {
                    Sort(list, comparer, pivot + 1, right);
                    right = pivot - 1;
                }
            }
        }

        private int Partition(IList<T> list, IComparer<T> comparer, int left, int mid, int right)
        {
            MidThree(list, comparer, left, mid, right);
            SortUtils.Swap(list, mid, left);

            T pivotValue = list[left];
            int l = left, r = right;
            while (l < r)
            {
                while (l < r && comparer.Compare(list[r], pivotValue) >= 0) // 右侧数比pivot大（包括等于），合规跳过
                {
                    r--;
                }
                if (l < r)
                {
                    list[l] = list[r];
                    l++;
                }

                while (l < r && comparer.Compare(list[l], pivotValue) <= 0)
                {
                    l++;
                }
                if (l < r)
                {
                    list[r] = list[l];
                    r--;
                }
            }

            list[l] = pivotValue;
            return l;
        }

        private void MidThree(IList<T> list, IComparer<T> comparer, int left, int mid, int right)
        {
            // 直接把三个位置局部排好：left <= mid <= right
            if (comparer.Compare(list[left], list[mid]) > 0)
                SortUtils.Swap(list, left, mid);

            if (comparer.Compare(list[left], list[right]) > 0)
                SortUtils.Swap(list, left, right);

            if (comparer.Compare(list[mid], list[right]) > 0)
                SortUtils.Swap(list, mid, right);
        }
    }
}
