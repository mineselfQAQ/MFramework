using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MFramework;
using MFramework.DLC;

public class Test_MDictionary : MonoBehaviour
{
    private void Start()
    {
        MDictionary<string, int> dic = new MDictionary<string, int>();

        dic["A"] = 1;
        dic["B"] = 2;
        dic.Add("C", 3);

        MLog.Print("KeyValuePair:");
        foreach (KeyValuePair<string, int> pair in dic)
        {
            MLog.Print($"Key: {pair.Key}  Value: {pair.Value}");
        }
        MLog.Print("Key:");
        foreach (string key in dic.Keys)
        {
            MLog.Print($"Key: {key}");
        }
        MLog.Print("Value:");
        foreach (int value in dic.Values)
        {
            MLog.Print($"Value: {value}");
        }

        MLog.Print(MLog.ColorWord("---分隔符---", Color.red));

        dic.Remove("B");
        MLog.Print($"dic[\"A\"]: {dic["A"]}");
        //MLog.Print($"dic[\"B\"]: {dic["B"]}");//报错，因为Key'B'已经不存在
        MLog.Print($"Count: {dic.Count}");
        MLog.Print($"字典中是否存在Key'B': {dic.ContainsKey("B")}");
        MLog.Print($"字典中是否存在Value'2': {dic.ContainsValue(2)}");
        KeyValuePair<string, int> kvPair = new KeyValuePair<string, int>("A", 1);
        MLog.Print($"字典中是否存在KeyValuePair'A'-'1': {dic.Contains(kvPair)}");

        dic.Clear();
    }
}
