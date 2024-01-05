using MFramework;
using UnityEngine;
using static MFramework.SortingAlgorithm;

public class Test_SortingAlgorithm : MonoBehaviour
{
    private void Start()
    {
        int[] arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        Log.Print(Log.ColorWord("—°‘Ò≈≈–ÚSelectionSort", Color.black, true));
        Print(arr);
        SelectionSort(arr);
        Print(arr);

        Log.Print(Log.ColorWord("---∑÷∏Ù∑˚---", Color.red));

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 }; 
        Log.Print(Log.ColorWord("√∞≈›≈≈–ÚBubbleSort", Color.black, true));
        Print(arr);
        BubbleSort(arr);
        Print(arr);

        Log.Print(Log.ColorWord("---∑÷∏Ù∑˚---", Color.red));

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        Log.Print(Log.ColorWord("≤Â»Î≈≈–ÚInsertionSort", Color.black, true));
        Print(arr);
        InsertionSort(arr);
        Print(arr);

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        Log.Print(Log.ColorWord("øÏÀŸ≈≈–ÚQuickSort", Color.black, true));
        Print(arr);
        QuickSort(arr);
        Print(arr);
    }

    private void Print(int[] arr)
    {
        string outputStr = "";
        foreach (int i in arr)
        {
            outputStr += $"{i} ";
        }
        Log.Print(outputStr);
    }
}
