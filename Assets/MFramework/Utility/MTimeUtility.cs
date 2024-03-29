using System;
using System.Collections;
using UnityEngine;

public static class MTimeUtility
{
    public static void Delay(float sec)
    {
        CoroutineHandler.Instance.BeginCoroutineAndNotRecord(DelayEnumerator(sec));
    }
    public static void Delay(Action action, float sec)
    {
        CoroutineHandler.Instance.BeginCoroutineAndNotRecord(DelayAndDoEnumerator(action, sec));
    }

    private static IEnumerator DelayEnumerator(float sec)
    {
        yield return new WaitForSeconds(sec);
    }

    private static IEnumerator DelayAndDoEnumerator(Action action, float sec)
    {
        yield return new WaitForSeconds(sec);
        action();
    }
}
