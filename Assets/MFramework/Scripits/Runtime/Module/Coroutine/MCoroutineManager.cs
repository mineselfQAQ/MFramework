using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        #region 无记录携程(用于无MonoBehaviour脚本)
        public Coroutine BeginCoroutineNoRecord(IEnumerator enumerator)
        {
            return StartCoroutine(enumerator);
        }
        public void EndCoroutine(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }
        #endregion

        #region 自我管理携程
        public Coroutine StartCoroutine(IEnumerator fun, string name, Action onFinish = null)
        {
            return StartCoroutine(StartCoroutinueRoutine(fun, name, onFinish));
        }

        public bool EndCoroutine(string name)
        {
            if (!dic.ContainsKey(name))
            {
                //MLog.Print($"{typeof(MCoroutineManager)}:字典中没有名为{name}的Coroutine，请检查.", MLogType.Warning);
                return false;
            }

            StopCoroutine(dic[name]);
            dic.Remove(name);
            count--;

            return true;
        }

        public void EndAllCoroutines()
        {
            foreach (var value in dic.Values)
            {
                StopCoroutine(value);
            }
            dic.Clear();
            count = 0;
        }

        private IEnumerator StartCoroutinueRoutine(IEnumerator enumerator, string name, Action onFinish)
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

            OnCoroutineFinishedInternal(name);
            onFinish?.Invoke();

            yield break;
        }

        private void OnCoroutineFinishedInternal(string name)
        {
            if (dic.ContainsKey(name))
            {
                dic.Remove(name);
                count--;
            }
        }
        #endregion

        #region 特殊携程
        public Coroutine DelayWithTimeScaleNoRecord(Action action, float interval)
        {
            return StartCoroutine(MCoroutineUtility.DelayWithTimeScale(action, interval));
        }

        /// <summary>
        /// 等待后执行(不记录)
        /// </summary>
        public Coroutine DelayNoRecord(Action action, float interval)
        {
            return StartCoroutine(MCoroutineUtility.Delay(action, interval));
        }
        /// <summary>
        /// 重复执行(不记录)
        /// </summary>
        public Coroutine RepeatNoRecord(Action action, bool startDo, int count, float interval, Action onFinish = null)
        {
            return StartCoroutine(MCoroutineUtility.Repeat(action, startDo, count, interval, onFinish));
        }
        /// <summary>
        /// 等待后重复执行(不记录)
        /// </summary>
        public Coroutine DelayRepeatNoRecord(Action action, float startInterval, int repeatCount, float repeatInterval, Action onFinish = null)
        {
            return StartCoroutine(MCoroutineUtility.DelayRepeat(action, startInterval, repeatCount, repeatInterval, onFinish));
        }
        /// <summary>
        /// 持续执行操作(不记录)
        /// </summary>
        public Coroutine LoopNoRecord(Action action, float startInterval, float repeatInterval)
        {
            return StartCoroutine(MCoroutineUtility.Loop(action, startInterval, repeatInterval));
        }

        /// <summary>
        /// 等待后执行
        /// </summary>
        public void Delay(string name, Action action, float interval)
        {
            StartCoroutine(MCoroutineUtility.Delay(action, interval), name);
        }
        /// <summary>
        /// 重复执行
        /// </summary>
        public void Repeat(string name, Action action, bool startDo, int count, float interval, Action onFinish = null)
        {
            StartCoroutine(MCoroutineUtility.Repeat(action, startDo, count, interval, onFinish), name);
        }
        /// <summary>
        /// 等待后重复执行
        /// </summary>
        public void DelayRepeat(string name, Action action, float startInterval, int repeatCount, float repeatInterval, Action onFinish = null)
        {
            StartCoroutine(MCoroutineUtility.DelayRepeat(action, startInterval, repeatCount, repeatInterval, onFinish), name);
        }
        /// <summary>
        /// 持续执行操作
        /// </summary>
        public void Loop(string name, Action action, float startInterval, float repeatInterval)
        {
            StartCoroutine(MCoroutineUtility.Loop(action, startInterval, repeatInterval), name);
        }

        /// <summary>
        /// 补间动画操作(不记录)
        /// </summary>
        internal Coroutine TweenNoRecord(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            return StartCoroutine(TweenRoutine(action, curve, duration, startValue, endValue, onFinish));
        }
        /// <summary>
        /// 补间动画操作
        /// </summary>
        internal void Tween(string name, Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            StartCoroutine(TweenRoutine(action, curve, duration, startValue, endValue, onFinish), name);
        }

        //internal static WaitForFixedUpdate waitFixedUpdate = new WaitForFixedUpdate();//受TimeScale影响
        //internal static WaitForSecondsRealtime waitFixedUpdate = new WaitForSecondsRealtime(Time.fixedDeltaTime);
        internal IEnumerator TweenRoutine(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            float step = duration / Time.fixedDeltaTime;//执行次数
            float length = endValue - startValue;//区间长度

            float curValue;
            for (int i = 0; i < step; i++)
            {
                curValue = startValue + MCurveSampler.Sample(curve, i / step) * length;
                action.Invoke(curValue);

                yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
            }
            curValue = curve.curveDir == CurveDir.Increment ? endValue : startValue;
            action.Invoke(curValue);

            onFinish?.Invoke();
        }
        #endregion
    }
}