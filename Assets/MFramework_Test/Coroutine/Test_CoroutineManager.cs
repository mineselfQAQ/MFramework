using MFramework;
using System.Collections;
using UnityEngine;

public class Test_CoroutineManager : MonoBehaviour
{
    private void Start()
    {
        CoroutineManager.Instance.BeginCoroutine(TestCoroutine(), "Coroutine1");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            MLog.Print(CoroutineManager.Instance.Count);
            CoroutineManager.Instance.EndCoroutine("Coroutine1");
        }
    }

    IEnumerator TestCoroutine()
    {
        MLog.Print("携程开始");

        yield return new WaitForSeconds(5);

        MLog.Print("携程结束");
    }
}
