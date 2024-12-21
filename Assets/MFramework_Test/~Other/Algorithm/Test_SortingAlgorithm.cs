using MFramework;
using UnityEngine;
using static MFramework.DLC.SortingAlgorithm;

public class Test_SortingAlgorithm : MonoBehaviour
{
    private void Start()
    {
        int[] arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        MLog.Print(MLog.Bold("ѡ������SelectionSort"));
        Print(arr);
        SelectionSort(arr);
        Print(arr);

        MLog.Print(MLog.Color("---�ָ���---", Color.red));

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 }; 
        MLog.Print(MLog.Bold("ð������BubbleSort"));
        Print(arr);
        BubbleSort(arr);
        Print(arr);

        MLog.Print(MLog.Color("---�ָ���---", Color.red));

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        MLog.Print(MLog.Bold("��������InsertionSort"));
        Print(arr);
        InsertionSort(arr);
        Print(arr);

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        MLog.Print(MLog.Bold("��������QuickSort"));
        Print(arr);
        QuickSort(arr);
        Print(arr);

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        MLog.Print(MLog.Bold("�鲢����MergeSort"));
        Print(arr);
        MergeSort(arr);
        Print(arr);

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        MLog.Print(MLog.Bold("������HeapSort"));
        Print(arr);
        HeapSort(arr);
        Print(arr);

        float[] arr2 = new float[9] { 0.3f, 0.5f, 0.2f, 0.9f, 0.8f, 0.6f, 0.1f, 0.7f, 0.4f };
        MLog.Print(MLog.Bold("������HeapSort"));
        Print(arr2);
        BucketSort(arr2);
        Print(arr2);

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        MLog.Print(MLog.Bold("��������CountingSort"));
        Print(arr);
        CountingSort(arr);
        Print(arr);

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        MLog.Print(MLog.Bold("��������CountingSort"));
        Print(arr);
        RadixSort(arr);
        Print(arr);

        arr = new int[9] { 3, 5, 2, 9, 8, 6, 1, 7, 4 };
        MLog.Print(MLog.Bold("ϣ������CountingSort"));
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
        MLog.Print(outputStr);
    }
}
