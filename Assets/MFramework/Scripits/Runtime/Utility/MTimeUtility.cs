using System;
using System.Collections;
using UnityEngine;

namespace MFramework
{
    public static class MTimeUtility
    {
        //���Ƿǳ�����������ֱ��д������
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