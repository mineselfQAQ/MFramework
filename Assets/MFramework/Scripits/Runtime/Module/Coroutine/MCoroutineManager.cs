using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MFramework
{
    [MonoSingletonSetting(HideFlags.NotEditable, "#MCoroutineManager#")]
    public class MCoroutineManager : MonoSingleton<MCoroutineManager>
    {
        private Dictionary<string, Coroutine> dic;//Key---名字 Value---Coroutine实例

        private int count;
        public int Count
        {
            get
            {
                return count;
            }
        }

        private MCoroutineManager()
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

        public void BeginCoroutine(IEnumerator fun, string name, Action onFinish = null)
        {
            StartCoroutine(BeginCoroutinueInternal(fun, name, onFinish));
        }

        public bool EndCoroutine(string name)
        {
            if (!dic.ContainsKey(name))
            {
                MLog.Print($"{typeof(MCoroutineManager)}:字典中没有名为{name}的Coroutine，请检查.", MLogType.Warning);
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

        private IEnumerator BeginCoroutinueInternal(IEnumerator enumerator, string name, Action onFinish)
        {
            if (dic.ContainsKey(name))
            {
                MLog.Print($"{typeof(MCoroutineManager)}：{name}已存在，请检查", MLogType.Warning);
                yield break;
            }

            Coroutine coroutine = StartCoroutine(enumerator);
            dic.Add(name, coroutine);
            count++;

            yield return coroutine;

            OnCoroutineFinished(name);
            onFinish?.Invoke();

            yield break;
        }

        private void OnCoroutineFinished(string name)
        {
            if (dic.ContainsKey(name))
            {
                dic.Remove(name);
                count--;
            }
        }

        public void BeginCoroutineNoRecord(IEnumerator enumerator)
        {
            StartCoroutine(enumerator);
        }
        /// <summary>
        /// 等待后执行
        /// 延迟<interval>秒后执行操作
        /// </summary>
        public void DelayNoRecord(Action action, float interval)
        {
            StartCoroutine(MCoroutineUtility.Delay(action, interval));
        }
        /// <summary>
        /// 重复执行
        /// 每过<interval>秒后执行操作，共执行count次(startDo会调用后直接执行一次)
        /// </summary>
        public void RepeatNoRecord(Action action, bool startDo, int count, float interval, Action onFinish = null)
        {
            StartCoroutine(MCoroutineUtility.Repeat(action, startDo, count, interval, onFinish));
        }
        /// <summary>
        /// 等待后重复执行
        /// 经过<startInterval>秒后执行第一次操作，
        /// 然后每过<interval>秒后执行操作，共执行count-1次
        /// </summary>
        public void DelayRepeatNoRecord(Action action, float startInterval, int repeatCount, float repeatInterval, Action onFinish = null)
        {
            StartCoroutine(MCoroutineUtility.DelayRepeat(action, startInterval, repeatCount, repeatInterval, onFinish));
        }
        /// <summary>
        /// 持续执行操作
        /// 经过<startInterval>秒后执行第一次操作，
        /// 然后每过<interval>秒后执行操作，无限执行
        /// </summary>
        public void LoopNoRecord(Action action, float startInterval, float repeatInterval)
        {
            StartCoroutine(MCoroutineUtility.Loop(action, startInterval, repeatInterval));
        }
        
        public void Delay(string name, Action action, float interval)
        {
            BeginCoroutine(MCoroutineUtility.Delay(action, interval), name);
        }
        public void Repeat(string name, Action action, bool startDo, int count, float interval, Action onFinish = null)
        {
            BeginCoroutine(MCoroutineUtility.Repeat(action, startDo, count, interval, onFinish), name);
        }
        public void DelayRepeat(string name, Action action, float startInterval, int repeatCount, float repeatInterval, Action onFinish = null)
        {
            BeginCoroutine(MCoroutineUtility.DelayRepeat(action, startInterval, repeatCount, repeatInterval, onFinish), name);
        }

        //=====MTween=====
        internal static WaitForFixedUpdate waitFixedUpdate = new WaitForFixedUpdate();

        internal void TweenNoRecord(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action<float> onFinish)
        {
            StartCoroutine(Tween(action, curve, duration, startValue, endValue, onFinish));
        }
        internal IEnumerator Tween(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action<float> onFinish)
        {
            float step = duration / Time.fixedDeltaTime;//执行次数
            float length = endValue - startValue;//区间长度

            float curValue;
            for (int i = 0; i < step; i++)
            {
                curValue = startValue + MCurveSampler.Sample(curve, i / step) * length;
                action.Invoke(curValue);

                yield return waitFixedUpdate;
            }
            curValue = curve.curveDir == CurveDir.Increment ? endValue : startValue;
            action.Invoke(curValue);

            onFinish?.Invoke(curValue);
        }
    }
}