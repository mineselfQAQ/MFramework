using MFramework;
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

    public Coroutine BeginCoroutine(IEnumerator fun, string name = null)
    {
        Coroutine coroutine = StartCoroutine(fun);
        count++;

        if (name == null)
        {
            name = count.ToString();
        }
        dic.Add(name, coroutine);

        return coroutine;
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

    public bool EndCoroutine(Coroutine coroutine)
    {
        //var keys = dic.Where(q => q.Value == coroutine).Select(q => q.Key);
        var key = dic.FirstOrDefault(q => q.Value == coroutine).Key;
        if (key == null)
        {
            return false;
        }

        StopCoroutine(dic[name]);
        dic.Remove(name);
        count--;

        return true;
    }

    public void EndAllCoroutine(Coroutine coroutine)
    {
        foreach (var value in dic.Values)
        {
            StopCoroutine(value);
        }
        dic.Clear();
        count = 0;
    }
}
