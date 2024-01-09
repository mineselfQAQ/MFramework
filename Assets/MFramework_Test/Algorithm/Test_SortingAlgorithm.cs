using MFramework;
using UnityEditor;
using UnityEngine;
using static MFramework.SortingAlgorithm;

public class Test_SortingAlgorithm : MonoBehaviour
{
    private void Start()
    {
        int[] arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        Log.Print(Log.BoldWord("—°‘Ò≈≈–ÚSelectionSort"));
        Print(arr);
        SelectionSort(arr);
        Print(arr);

        Log.Print(Log.ColorWord("---∑÷∏Ù∑˚---", Color.red));

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 }; 
        Log.Print(Log.BoldWord("√∞≈›≈≈–ÚBubbleSort"));
        Print(arr);
        BubbleSort(arr);
        Print(arr);

        Log.Print(Log.ColorWord("---∑÷∏Ù∑˚---", Color.red));

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        Log.Print(Log.BoldWord("≤Â»Î≈≈–ÚInsertionSort"));
        Print(arr);
        InsertionSort(arr);
        Print(arr);

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        Log.Print(Log.BoldWord("øÏÀŸ≈≈–ÚQuickSort"));
        Print(arr);
        QuickSort(arr);
        Print(arr);

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        Log.Print(Log.BoldWord("πÈ≤¢≈≈–ÚMergeSort"));
        Print(arr);
        MergeSort(arr);
        Print(arr);

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        Log.Print(Log.BoldWord("∂—≈≈–ÚHeapSort"));
        Print(arr);
        HeapSort(arr);
        Print(arr);

        float[] arr2 = new float[9] { 0.3f, 0.5f, 0.2f, 0.9f, 0.8f, 0.6f, 0.1f, 0.7f, 0.4f };
        Log.Print(Log.BoldWord("∂—≈≈–ÚHeapSort"));
        Print(arr2);
        BucketSort(arr2);
        Print(arr2);

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        Log.Print(Log.BoldWord("º∆ ˝≈≈–ÚCountingSort"));
        Print(arr);
        CountingSort(arr);
        Print(arr);

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        Log.Print(Log.BoldWord("ª˘ ˝≈≈–ÚCountingSort"));
        Print(arr);
        RadixSort(arr);
        Print(arr);

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        Log.Print(Log.BoldWord("œ£∂˚≈≈–ÚCountingSort"));
        Print(arr);
        ShellSort(arr);
        Print(arr);
    }

    private void Print<T>(T[] arr)
    {
        string outputStr = "";
        foreach (T i in arr)
        {
            outputStr += $"{i} ";
        }
        Log.Print(outputStr);
    }
}
