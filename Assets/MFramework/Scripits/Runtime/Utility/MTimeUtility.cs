using System;
using System.Collections;
using UnityEngine;

namespace MFramework
{
    public static class MTimeUtility
    {
        //不是非常清晰，不如直接写在外面
        public static void LoopChecker(bool switcher, ref float totalTime, float duration, Action onFinish)
        {
            if (switcher)
            {
                totalTime += Time.deltaTime;

                if (totalTime >= duration)
                {
                    totalTime = 0;
                    onFinish?.Invoke();
                }
            }
        }
    }
}