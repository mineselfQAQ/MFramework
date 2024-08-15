using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    [MonoSingletonSetting(HideFlags.NotEditable, "#MCoroutineManager#")]
    public class MCoroutineManager : MonoSingleton<MCoroutineManager>
    {
        private Dictionary<string, Coroutine> dic;//Key---���� Value---Coroutineʵ��

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

        #region �޼�¼Я��(������MonoBehaviour�ű�)
        public Coroutine BeginCoroutineNoRecord(IEnumerator enumerator)
        {
            return StartCoroutine(enumerator);
        }
        public void EndCoroutine(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }
        #endregion

        #region ���ҹ���Я��
        public void StartCoroutine(IEnumerator fun, string name, Action onFinish = null)
        {
            StartCoroutine(StartCoroutinueRoutine(fun, name, onFinish));
        }

        public bool EndCoroutine(string name)
        {
            if (!dic.ContainsKey(name))
            {
                //MLog.Print($"{typeof(MCoroutineManager)}:�ֵ���û����Ϊ{name}��Coroutine������.", MLogType.Warning);
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
                MLog.Print($"{typeof(MCoroutineManager)}��{name}�Ѵ��ڣ�����", MLogType.Warning);
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

        #region ����Я��
        public Coroutine DelayWithTimeScaleNoRecord(Action action, float interval)
        {
            return StartCoroutine(MCoroutineUtility.DelayWithTimeScale(action, interval));
        }

        /// <summary>
        /// �ȴ���ִ��(����¼)
        /// </summary>
        public Coroutine DelayNoRecord(Action action, float interval)
        {
            return StartCoroutine(MCoroutineUtility.Delay(action, interval));
        }
        /// <summary>
        /// �ظ�ִ��(����¼)
        /// </summary>
        public Coroutine RepeatNoRecord(Action action, bool startDo, int count, float interval, Action onFinish = null)
        {
            return StartCoroutine(MCoroutineUtility.Repeat(action, startDo, count, interval, onFinish));
        }
        /// <summary>
        /// �ȴ����ظ�ִ��(����¼)
        /// </summary>
        public Coroutine DelayRepeatNoRecord(Action action, float startInterval, int repeatCount, float repeatInterval, Action onFinish = null)
        {
            return StartCoroutine(MCoroutineUtility.DelayRepeat(action, startInterval, repeatCount, repeatInterval, onFinish));
        }
        /// <summary>
        /// ����ִ�в���(����¼)
        /// </summary>
        public Coroutine LoopNoRecord(Action action, float startInterval, float repeatInterval)
        {
            return StartCoroutine(MCoroutineUtility.Loop(action, startInterval, repeatInterval));
        }

        /// <summary>
        /// �ȴ���ִ��
        /// </summary>
        public void Delay(string name, Action action, float interval)
        {
            StartCoroutine(MCoroutineUtility.Delay(action, interval), name);
        }
        /// <summary>
        /// �ظ�ִ��
        /// </summary>
        public void Repeat(string name, Action action, bool startDo, int count, float interval, Action onFinish = null)
        {
            StartCoroutine(MCoroutineUtility.Repeat(action, startDo, count, interval, onFinish), name);
        }
        /// <summary>
        /// �ȴ����ظ�ִ��
        /// </summary>
        public void DelayRepeat(string name, Action action, float startInterval, int repeatCount, float repeatInterval, Action onFinish = null)
        {
            StartCoroutine(MCoroutineUtility.DelayRepeat(action, startInterval, repeatCount, repeatInterval, onFinish), name);
        }
        /// <summary>
        /// ����ִ�в���
        /// </summary>
        public void Loop(string name, Action action, float startInterval, float repeatInterval)
        {
            StartCoroutine(MCoroutineUtility.Loop(action, startInterval, repeatInterval), name);
        }

        /// <summary>
        /// ���䶯������(����¼)
        /// </summary>
        internal Coroutine TweenNoRecord(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            return StartCoroutine(TweenRoutine(action, curve, duration, startValue, endValue, onFinish));
        }
        /// <summary>
        /// ���䶯������
        /// </summary>
        internal void Tween(string name, Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            StartCoroutine(TweenRoutine(action, curve, duration, startValue, endValue, onFinish), name);
        }

        internal static WaitForFixedUpdate waitFixedUpdate = new WaitForFixedUpdate();
        internal IEnumerator TweenRoutine(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish)
        {
            float step = duration / Time.fixedDeltaTime;//ִ�д���
            float length = endValue - startValue;//���䳤��

            float curValue;
            for (int i = 0; i < step; i++)
            {
                curValue = startValue + MCurveSampler.Sample(curve, i / step) * length;
                action.Invoke(curValue);

                yield return waitFixedUpdate;
            }
            curValue = curve.curveDir == CurveDir.Increment ? endValue : startValue;
            action.Invoke(curValue);

            onFinish?.Invoke();
        }
        #endregion
    }
}