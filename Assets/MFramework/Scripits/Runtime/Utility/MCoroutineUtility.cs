using System;
using System.Collections;
using UnityEngine;

namespace MFramework
{
    internal class MCoroutineUtility
    {
        internal static IEnumerator Begin(Action action)
        {
            action();
            yield break;
        }

        internal static IEnumerator Delay(Action action, float interval)
        {
            yield return new WaitForSecondsRealtime(interval);
            action();
        }

        internal static IEnumerator Repeat(Action action, bool startDo, int repeatCount, float repeatInterval, Action onFinish)
        {
            //虽然称为Repeat()，但是repeatCount=0也是可以接受的(如一个循环中有时重复10次，有时只执行1次)

            if (startDo)
            {
                action();
                repeatCount--;
            }

            for (int i = 0; i < repeatCount; i++)
            {
                yield return new WaitForSecondsRealtime(repeatInterval);
                action();
            }

            onFinish?.Invoke();
        }

        internal static IEnumerator DelayRepeat(Action action, float startInterval, int repeatCount, float repeatInterval, Action onFinish)
        {
            //虽然称为DelayRepeat()，但是repeatCount=0也是可以接受的(如一个循环中有时重复10次，有时只执行1次)

            yield return new WaitForSecondsRealtime(startInterval);
            action();

            for (int i = 1; i < repeatCount; i++)
            {
                yield return new WaitForSecondsRealtime(repeatInterval);
                action();
            }

            onFinish?.Invoke();
        }

        internal static IEnumerator Loop(Action action, float startInterval, float repeatInterval)
        {
            yield return new WaitForSecondsRealtime(startInterval);

            while (true)
            {
                action();

                yield return new WaitForSecondsRealtime(repeatInterval);
            }
        }
    }
}

