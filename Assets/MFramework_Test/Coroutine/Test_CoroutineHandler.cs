using MFramework;
using System.Collections;
using UnityEngine;

public class Test_CoroutineHandler : MonoBehaviour
{
    private void Start()
    {
        CoroutineHandler.Instance.BeginCoroutine(TestCoroutine(), "Coroutine1");
    }

    private void Update()
    {
        MLog.Print(CoroutineHandler.Instance.Count);

        if (Input.GetKeyDown(KeyCode.E))
        {
            CoroutineHandler.Instance.EndCoroutine("Coroutine1");
        }
    }

    IEnumerator TestCoroutine()
    {
        MLog.Print("携程开始");

        yield return new WaitForSeconds(5);

        MLog.Print("携程结束");
    }
}
