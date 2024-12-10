using System;
using System.Collections;
using UnityEngine;

namespace MFramework
{
    internal class MCoroutineUtility
    {
        internal static IEnumerator DelayFrame(Action action, int frame)
        {
            for(int i = 0; i < frame; i++) yield return null;

            action();
        }

        internal static IEnumerator DelayWithTimeScale(Action action, float interval)
        {
            yield return new WaitForSeconds(interval);
            action();
        }

        internal static IEnumerator Do(Action action)
        {
            action();
            yield return null;
        }
        internal static IEnumerator DoWithCallBack(Action action, Action onFinish)
        {
            yield return DoWithCallBack(() =>
            {
                action?.Invoke();
                return null;
            }, onFinish);
        }
        private static IEnumerator DoWithCallBack(Func<IEnumerator> action, Action onFinish)
        {
            yield return action();

            onFinish();
        }

        /// <summary>     
        /// 延迟'interval'秒后执行操作
        /// </summary>
        internal static IEnumerator Delay(Action action, float interval)
        {
            yield return new WaitForSecondsRealtime(interval);
            action();
        }

        /// <summary>
        /// 每过'interval'秒后执行操作，共执行count次(startDo会调用后直接执行一次)
        /// </summary>
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

        /// <summary>        
        /// 经过'startInterval'秒后执行第一次操作，
        /// 然后每过'interval'秒后执行操作，共执行count-1次
        /// </summary>
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

        /// <summary>
        /// 经过'startInterval'秒后执行第一次操作，
        /// 然后每过'interval'秒后执行操作，无限执行
        /// </summary>
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

