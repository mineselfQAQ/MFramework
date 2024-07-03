using System;
using System.Collections;
using UnityEngine;

namespace MFramework
{
    public static class MTimeUtility
    {
        public static void Delay(float sec)
        {
            MCoroutineManager.Instance.BeginCoroutineNoRecord(DelayEnumerator(sec));
        }
        public static void Delay(Action action, float sec)
        {
            MCoroutineManager.Instance.BeginCoroutineNoRecord(DelayAndDoEnumerator(action, sec));
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
}