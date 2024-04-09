using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFramework;

public class Test_MDictionary : MonoBehaviour
{
    void Start()
    {
        MDictionary<int, int> dic = new MDictionary<int, int>();
        dic.Add(1, 1);
        dic.Add(2, 2);
        dic.Add(3, 3);
        MLog.Print(dic[1]);
        MLog.Print(dic[2]);
        MLog.Print(dic[3]);
    }
}
