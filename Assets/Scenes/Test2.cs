using System;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log(1);
        throw new Exception("错误");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(2);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(3);
    }

    void OnDestroy()
    {
        Debug.Log(4);
    }
}
