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
        hashtable.Remove(1);

        Log.Print("hashtable[1]是否有值: " + !(hashtable[1] == null));
        Log.Print("Count: " + hashtable.Count);
        Log.Print("是否有Key-2: " + hashtable.ContainsKey(2));
        Log.Print("是否有Value-元素2: " + hashtable.ContainsValue("元素2"));



        Log.Print(Log.ColorWord("---分隔符---", Color.red));



        Log.Print("输出Key:");
        string keyOutput = "";
        foreach (var item in hashtable.Keys)
        {
            keyOutput += $"{item} ";
        }
        Log.Print(keyOutput);



        Log.Print(Log.ColorWord("---分隔符---", Color.red));



        Log.Print("输出Value:");
        string valueOutput = "";
        foreach (var item in hashtable.Values)
        {
            valueOutput += $"{item} ";
        }
        Log.Print(valueOutput);



        Log.Print(Log.ColorWord("---分隔符---", Color.red));



        Log.Print("输出KeyValuePair:");
        string pairOutput = "";
        foreach (DictionaryEntry item in hashtable)
        {
            pairOutput += $"{item.Key}+{item.Value} ";
        }
        Log.Print(pairOutput);
    }
}
