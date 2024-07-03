using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var cr = StartCoroutine(a());


    }

    IEnumerator a()
    {
        yield return new WaitForSeconds(2);
        Coroutine cr2 = StartCoroutine(b());

        yield return new WaitForSeconds(2);

        Debug.Log("OK");
    }

    IEnumerator b()
    {
        yield return new WaitForSeconds(2);
    }
}