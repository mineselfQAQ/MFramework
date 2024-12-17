using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    //TODO:name形式应该分组，否则无法简单地满足Stop需求
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
        [Obsolete("无意义，内部不能使用协程，请直接使用StartCoroutine()")]
        public Coroutine BeginCoroutineNoRecord(Action action)
        {
            return StartCoroutine(MCoroutineUtility.Do(action));
        }
        public Coroutine BeginCoroutineWithCallBackNoRecord(Action action, Action onFinish)
        {
            return StartCoroutine(MCoroutineUtility.DoWithCallBack(action, onFinish));
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
        public Coroutine DelayFrame(Action action, int frame)
        {
            return StartCoroutine(MCoroutineUtility.DelayFrame(action, frame));
        }
        public Coroutine DelayOneFrame(Action action)
        {
            return StartCoroutine(MCoroutineUtility.DelayFrame(action, 1));
        }

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
        internal Coroutine UnscaledFixedTweenNoRecord(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            return StartCoroutine(UnsacledFixedTweenRoutine(action, curve, duration, startValue, endValue, onFinish));
        }
        /// <summary>
        /// 补间动画操作
        /// </summary>
        internal void UnscaledFixedTween(string name, Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            StartCoroutine(UnsacledFixedTweenRoutine(action, curve, duration, startValue, endValue, onFinish), name);
        }

        /// <summary>
        /// 补间动画操作(不记录)
        /// </summary>
        internal Coroutine FixedTweenNoRecord(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            return StartCoroutine(FixedTweenRoutine(action, curve, duration, startValue, endValue, onFinish));
        }
        /// <summary>
        /// 补间动画操作
        /// </summary>
        internal void FixedTween(string name, Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            StartCoroutine(FixedTweenRoutine(action, curve, duration, startValue, endValue, onFinish), name);
        }

        /// <summary>
        /// 补间动画操作(不记录)
        /// </summary>
        internal Coroutine UnscaledTweenNoRecord(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            return StartCoroutine(UnscaledTweenRoutine(action, curve, duration, startValue, endValue, onFinish));
        }
        /// <summary>
        /// 补间动画操作
        /// </summary>
        internal void UnsacledTween(string name, Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            StartCoroutine(UnscaledTweenRoutine(action, curve, duration, startValue, endValue, onFinish), name);
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

        /// <summary>
        /// 基于FixedDeltaTime的Tween动画(不受timeScale影响)
        /// </summary>
        internal IEnumerator UnsacledFixedTweenRoutine(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            float step = duration / Time.fixedUnscaledDeltaTime;//执行次数  不受timeScale影响
            float length = endValue - startValue;//区间长度

            float curValue;
            for (int i = 0; i < step; i++)
            {
                curValue = startValue + MCurveSampler.Sample(curve, i / step) * length;
                action.Invoke(curValue);

                //模拟一次FixedTime
                yield return new WaitForSecondsRealtime(Time.fixedUnscaledDeltaTime);
            }
            curValue = curve.curveDir == CurveDir.Increment ? endValue : startValue;
            action.Invoke(curValue);

            onFinish?.Invoke();
        }
        /// <summary>
        /// 基于FixedDeltaTime的Tween动画(受timeScale影响)
        /// </summary>
        internal IEnumerator FixedTweenRoutine(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            float step = duration / Time.fixedUnscaledDeltaTime;//执行次数  不受timeScale影响
            float length = endValue - startValue;//区间长度

            float curValue;
            for (int i = 0; i < step; i++)
            {
                curValue = startValue + MCurveSampler.Sample(curve, i / step) * length;
                action.Invoke(curValue);

                //模拟一次FixedTime
                yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
            }
            curValue = curve.curveDir == CurveDir.Increment ? endValue : startValue;
            action.Invoke(curValue);

            onFinish?.Invoke();
        }
        /// <summary>
        /// 基于DeltaTime的Tween动画(不受timeScale影响)
        /// </summary>
        internal IEnumerator UnscaledTweenRoutine(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            float elapsed = 0.0f;//经过时长
            float length = endValue - startValue;//区间长度

            float curValue;
            while (elapsed < duration)
            {
                float progress = Mathf.Clamp01(elapsed / duration);//进度[0,1]
                curValue = startValue + MCurveSampler.Sample(curve, progress) * length;
                action.Invoke(curValue);

                elapsed += Time.unscaledDeltaTime;//不受timeScale影响
                yield return null;
            }
            //最后一帧
            curValue = curve.curveDir == CurveDir.Increment ? endValue : startValue;
            action.Invoke(curValue);

            onFinish?.Invoke();
        }
        /// <summary>
        /// 基于DeltaTime的Tween动画(受timeScale影响)
        /// </summary>
        internal IEnumerator TweenRoutine(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            float elapsed = 0.0f;//经过时长
            float length = endValue - startValue;//区间长度

            float curValue;
            while (elapsed < duration)
            {
                float progress = Mathf.Clamp01(elapsed / duration);//进度[0,1]
                curValue = startValue + MCurveSampler.Sample(curve, progress) * length;
                action.Invoke(curValue);

                elapsed += Time.deltaTime;//受timeScale影响
                yield return null;
            }
            //最后一帧
            curValue = curve.curveDir == CurveDir.Increment ? endValue : startValue;
            action.Invoke(curValue);

            onFinish?.Invoke();
        }
        #endregion
    }
}