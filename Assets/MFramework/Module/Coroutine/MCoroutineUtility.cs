using System;
using System.Collections;
using UnityEngine;

namespace MFramework.Coroutines
{
    internal static class MCoroutineUtility
    {
        internal static IEnumerator DelayFrame(Action action, int frame)
        {
            if (frame < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(frame));
            }

            for (int i = 0; i < frame; i++)
            {
                yield return null;
            }

            action?.Invoke();
        }

        internal static IEnumerator DelayWithTimeScale(Action action, float interval)
        {
            yield return new WaitForSeconds(interval);
            action?.Invoke();
        }

        internal static IEnumerator Do(Action action)
        {
            action?.Invoke();
            yield return null;
        }

        internal static IEnumerator Wait(Action onFinish, BoolWrapper flag, int moreFrame)
        {
            if (flag == null)
            {
                throw new ArgumentNullException(nameof(flag));
            }

            if (moreFrame < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(moreFrame));
            }

            while (!flag.Value)
            {
                yield return null;
            }

            for (int i = 0; i < moreFrame; i++)
            {
                yield return null;
            }

            onFinish?.Invoke();
        }

        internal static IEnumerator Delay(Action action, float interval)
        {
            yield return new WaitForSecondsRealtime(interval);
            action?.Invoke();
        }

        internal static IEnumerator Repeat(Action action, bool startDo, int repeatCount, float repeatInterval)
        {
            if (repeatCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(repeatCount));
            }

            if (startDo)
            {
                action?.Invoke();
                repeatCount--;
            }

            for (int i = 0; i < repeatCount; i++)
            {
                yield return new WaitForSecondsRealtime(repeatInterval);
                action?.Invoke();
            }
        }

        internal static IEnumerator DelayRepeat(Action action, float startInterval, int repeatCount, float repeatInterval)
        {
            if (repeatCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(repeatCount));
            }

            if (repeatCount == 0)
            {
                yield break;
            }

            yield return new WaitForSecondsRealtime(startInterval);
            action?.Invoke();

            for (int i = 1; i < repeatCount; i++)
            {
                yield return new WaitForSecondsRealtime(repeatInterval);
                action?.Invoke();
            }
        }

        internal static IEnumerator Loop(Action action, float startInterval, float repeatInterval)
        {
            yield return new WaitForSecondsRealtime(startInterval);

            while (true)
            {
                action?.Invoke();
                yield return new WaitForSecondsRealtime(repeatInterval);
            }
        }
    }
}
