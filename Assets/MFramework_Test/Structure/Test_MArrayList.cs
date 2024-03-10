using MFramework;
using UnityEngine;

public class Test_MArrayList : MonoBehaviour
{
    private void Start()
    {
        MArrayList list = new MArrayList();

        list.Add(5);//5
        list.Add(6);//5 6
        list.Add(7);//5 6 7
        list.Add(8);//5 6 7 8

        MLog.Print($"Capacity: {list.Capacity} Count: {list.Count}");

        list.Add(9);//5 6 7 8 9
        MLog.Print($"Before TrimToSize: Capacity: {list.Capacity} Count: {list.Count}");
        list.TrimToSize();
        MLog.Print($"After TrimToSize: Capacity: {list.Capacity} Count: {list.Count}");

        MLog.Print($"List contain 6: {list.Contains(6)}");

        MLog.Print(MLog.ColorWord("---·Ö¸ô·û---", Color.red));

        list.Insert(2, 10);//5 6 10 7 8 9
        list.RemoveAt(5);//5 6 10 7 8

        list.Print();

        list.Sort();

        list.Print();

        list.Reverse();

        list.Print();

        list.Clear();

        list.Print();
    }
}
