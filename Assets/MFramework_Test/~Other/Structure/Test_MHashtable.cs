using MFramework;
using System.Collections;
using UnityEngine;

public class Test_MHashtable : MonoBehaviour
{
    private void Start()
    {
        MHashtable hashtable = new MHashtable();

        hashtable.Add(1, "Ԫ��1");
        hashtable.Add(2, "Ԫ��2");
        hashtable.Add(3, "Ԫ��3");
        //for (int i = 0; i <= int.MaxValue; i+=10)
        //{
        //    hashtable.Add(i, "1");
        //}
        hashtable.Remove(1);

        MLog.Print("hashtable[1]�Ƿ���ֵ: " + !(hashtable[1] == null));
        MLog.Print("Count: " + hashtable.Count);
        MLog.Print("�Ƿ���Key-2: " + hashtable.ContainsKey(2));
        MLog.Print("�Ƿ���Value-Ԫ��2: " + hashtable.ContainsValue("Ԫ��2"));



        MLog.Print(MLog.ColorWord("---�ָ���---", Color.red));



        MLog.Print("���Key:");
        string keyOutput = "";
        foreach (var item in hashtable.Keys)
        {
            keyOutput += $"{item} ";
        }
        MLog.Print(keyOutput);



        MLog.Print(MLog.ColorWord("---�ָ���---", Color.red));



        MLog.Print("���Value:");
        string valueOutput = "";
        foreach (var item in hashtable.Values)
        {
            valueOutput += $"{item} ";
        }
        MLog.Print(valueOutput);



        MLog.Print(MLog.ColorWord("---�ָ���---", Color.red));



        MLog.Print("���KeyValuePair:");
        string pairOutput = "";
        foreach (DictionaryEntry item in hashtable)
        {
            pairOutput += $"{item.Key}+{item.Value} ";
        }
        MLog.Print(pairOutput);
    }
}
