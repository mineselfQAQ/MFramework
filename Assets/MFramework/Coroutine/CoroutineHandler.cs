using MFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoroutineHandler : MonoSingleton<CoroutineHandler>
{
    private Dictionary<string, Coroutine> dic;//Key---Ãû×Ö Value---CoroutineÊµÀý

    private int count;
    public int Count
    {
        get
        {
            return count;
        }
    }

    private CoroutineHandler()
    {
        dic = new Dictionary<string, Coroutine>();
    }

    private void OnApplicationQuit()
    {
        foreach (var value in dic.Values)
        {
            StopCoroutine(value);
        }
    }

    public void BeginCoroutine(IEnumerator fun, string name)
    {
        Coroutine coroutine = StartCoroutine(BeginCoroutinueInternal(fun, name));

        dic.Add(name, coroutine);
        count++;
    }

    private IEnumerator BeginCoroutinueInternal(IEnumerator enumerator, string name)
    {
        bool flag = false;
        Coroutine coroutine = StartCoroutine(enumerator);

        if (flag)
        {
            OnCoroutineFinished(name);

            yield break;
        }
    }

    private void OnCoroutineFinished(string name)
    {
        StopCoroutine(dic[name]);
        dic.Remove(name);
        count--;
    }

    public bool EndCoroutine(string name)
    {
        if (!dic.ContainsKey(name))
        {
            return false;
        }

        StopCoroutine(dic[name]);
        dic.Remove(name);
        count--;

        return true;
    }

    public void EndAllCoroutine()
    {
        foreach (var value in dic.Values)
        {
            StopCoroutine(value);
        }
        dic.Clear();
        count = 0;
    }
}
