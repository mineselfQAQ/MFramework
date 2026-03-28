using System.Collections.Generic;
using MFramework.Core;

using MFrameworkCSTests.Sort11;

using UnityEngine;

namespace MFrameworkCSTests.Sort
{
    public class Test_Sort : MonoBehaviour
    {
        private void Start()
        {
            Sort1<int> sort = new Sort1<int>();
            var l = GenerateList();
            sort.Sort(l, Comparer<int>.Default);
            l.Print();

            // SelectionSort<int> sort1 = new SelectionSort<int>();
            // var list = GenerateList();
            // sort1.Sort(list, Comparer<int>.Default, 0);
            // list.Print();
            // list = GenerateList();
            // sort1.Sort(list, Comparer<int>.Default, 1);
            // list.Print();
            //
            // BubbleSort<int> sort2 = new BubbleSort<int>();
            // list = GenerateList();
            // sort2.Sort(list, Comparer<int>.Default, 0);
            // list.Print();
            // list = GenerateList();
            // sort2.Sort(list, Comparer<int>.Default, 1);
            // list.Print();
            // list = GenerateList();
            // sort2.Sort(list, Comparer<int>.Default, 2);
            // list.Print();
            //
            // InsertionSort<int> sort3 = new InsertionSort<int>();
            // list = GenerateList();
            // sort3.Sort(list, Comparer<int>.Default);
            // list.Print();
            //
            // ShellSort<int> sort4 = new ShellSort<int>();
            // list = GenerateList();
            // sort4.Sort(list, Comparer<int>.Default);
            // list.Print();
            //
            // QuickSort<int> sort5 = new QuickSort<int>();
            // list = GenerateList();
            // sort5.Sort(list, Comparer<int>.Default, 0);
            // list.Print();
            // list = GenerateList();
            // sort5.Sort(list, Comparer<int>.Default, 1);
            // list.Print();
            // list = GenerateList();
            // sort5.Sort(list, Comparer<int>.Default, 2);
            // list.Print();
            // list = GenerateList();
            // sort5.Sort(list, Comparer<int>.Default, 3);
            // list.Print();
            // list = GenerateList();
            // sort5.Sort(list, Comparer<int>.Default, 4);
            // list.Print();
            //
            // MergeSort<int> sort6 = new MergeSort<int>();
            // list = GenerateList();
            // sort6.Sort(list, Comparer<int>.Default);
            //
            // HeapSort<int> sort7 = new HeapSort<int>();
            // list = GenerateList();
            // sort7.Sort(list, Comparer<int>.Default, 0);
            // list = GenerateList();
            // sort7.Sort(list, Comparer<int>.Default, 1);
        }

        private List<int> GenerateList()
        {
            return new List<int>{4, 1, 5, 2, 7, 8, 9, 6, 3};;
        }
    }

    public static class ListExtensions
    {
        public static void Print<T>(this IList<T> list)
        {
            Debug.Log(string.Join(" | ", list));
        }
    }
}
