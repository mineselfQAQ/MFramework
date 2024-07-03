using MFramework;
using MFramework.DLC;
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

        MLog.Print($"Capacity: {list.Capacity} Count: {list.Count}");
        MLog.Print($"Capacity: {list.Capacity} Count: {list.Count}");

        list.Add(9);//5 6 7 8 9
        MLog.Print($"Before TrimToSize: Capacity: {list.Capacity} Count: {list.Count}");
        list.TrimExcess();
        MLog.Print($"After TrimToSize: Capacity: {list.Capacity} Count: {list.Count}");

        MLog.Print($"List contain 6: {list.Contains(6)}");

        MLog.Print(MLog.ColorWord("---·Ö¸ô·û---", Color.red));

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
