using MFramework;
using System.Collections;
using UnityEngine;

public class Test_CoroutineManager : MonoBehaviour
{
    private void Start()
    {
        MCoroutineManager.Instance.BeginCoroutine(TestCoroutine(), "Coroutine1");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            MLog.Print(MCoroutineManager.Instance.Count);
            MCoroutineManager.Instance.EndCoroutine("Coroutine1");
        }
    }

    IEnumerator TestCoroutine()
    {
        MLog.Print("Я�̿�ʼ");

        yield return new WaitForSeconds(5);

        MLog.Print("Я�̽���");
    }
}
