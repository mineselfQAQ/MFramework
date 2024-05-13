using System;
using System.Collections;
using UnityEngine;

namespace MFramework
{
    public static class CoroutineUtility
    {
        public static IEnumerator Delay(Action onFinish, float duration)
        {
            yield return new WaitForSeconds(duration);
            onFinish();
        }
    }
}

