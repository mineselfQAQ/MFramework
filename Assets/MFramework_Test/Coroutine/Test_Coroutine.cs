using MFramework;
using System;
using System.Collections;
using UnityEngine;

public class Test_Coroutine : MonoBehaviour
{
    private void Start()
    {
        CoroutineHandler.Instance.BeginCoroutine(TestCoroutine(), "Coroutine1");
    }

    private void Update()
    {
        Debug.Log(CoroutineHandler.Instance.Count);

        if (Input.GetKeyDown(KeyCode.E))
        {
            CoroutineHandler.Instance.EndCoroutine("Coroutine1");
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
    }

    IEnumerator TestCoroutine()
    {
        MLog.Print("携程开始");

        yield return new WaitForSeconds(3);

        MLog.Print("携程结束");
    }
}
