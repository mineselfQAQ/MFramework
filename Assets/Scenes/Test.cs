using System;
using UnityEngine;

public class Test : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("我能执行");
    }

    void OnDestroy()
    {
        Debug.Log("我能执行");
    }
}
