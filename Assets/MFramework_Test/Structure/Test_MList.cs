using MFramework;
using UnityEngine;

public class Test_MList : MonoBehaviour
{
    private void Start()
    {
        MList<int> list = new MList<int>();

        list.Add(5);//5
        list.Add(6);//5 6
        list.Add(7);//5 6 7
        list.Add(8);//5 6 7 8

        Log.Print($"Capacity: {list.Capacity} Count: {list.Count}");
        Log.Print($"Capacity: {list.Capacity} Count: {list.Count}");

        list.Add(9);//5 6 7 8 9
        Log.Print($"Before TrimToSize: Capacity: {list.Capacity} Count: {list.Count}");
        list.TrimExcess();
        Log.Print($"After TrimToSize: Capacity: {list.Capacity} Count: {list.Count}");

        Log.Print($"List contain 6: {list.Contains(6)}");

        Log.Print(Log.ColorWord("---·Ö¸ô·û---", Color.red));

        list.Insert(2, 10);//5 6 10 7 8 9
        list.RemoveAt(5);//5 6 10 7 8

        list.Print();

        list.Sort();//5 6 7 8 10

        list.Print();

        list.Reverse();//10 8 7 6 5

        list.Print();

        list.Clear();

        list.Print();
    }
}
