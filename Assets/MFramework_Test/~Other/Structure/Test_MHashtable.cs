using MFramework;
using System.Collections;
using UnityEngine;

public class Test_MHashtable : MonoBehaviour
{
    private void Start()
    {
        MHashtable hashtable = new MHashtable();

        hashtable.Add(1, "元素1");
        hashtable.Add(2, "元素2");
        hashtable.Add(3, "元素3");
        //for (int i = 0; i <= int.MaxValue; i+=10)
        //{
        //    hashtable.Add(i, "1");
        //}
        hashtable.Remove(1);

        MLog.Print("hashtable[1]是否有值: " + !(hashtable[1] == null));
        MLog.Print("Count: " + hashtable.Count);
        MLog.Print("是否有Key-2: " + hashtable.ContainsKey(2));
        MLog.Print("是否有Value-元素2: " + hashtable.ContainsValue("元素2"));



        MLog.Print(MLog.ColorWord("---分隔符---", Color.red));



        MLog.Print("输出Key:");
        string keyOutput = "";
        foreach (var item in hashtable.Keys)
        {
            keyOutput += $"{item} ";
        }
        MLog.Print(keyOutput);



        MLog.Print(MLog.ColorWord("---分隔符---", Color.red));



        MLog.Print("输出Value:");
        string valueOutput = "";
        foreach (var item in hashtable.Values)
        {
            valueOutput += $"{item} ";
        }
        MLog.Print(valueOutput);



        MLog.Print(MLog.ColorWord("---分隔符---", Color.red));



        MLog.Print("输出KeyValuePair:");
        string pairOutput = "";
        foreach (DictionaryEntry item in hashtable)
        {
            pairOutput += $"{item.Key}+{item.Value} ";
        }
        MLog.Print(pairOutput);
    }
}
