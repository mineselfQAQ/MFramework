using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MFramework
{
    internal static class MTextAnimatorCoroutine
    {
        private static Dictionary<string, Coroutine> dic_TextAnimator = new Dictionary<string, Coroutine>();
        internal static Dictionary<TMP_Text, List<string>> IDs = new Dictionary<TMP_Text, List<string>>();//一个TMP_Text所执行的携程的字典Key

        internal static string GetAndAddID(TMP_Text text)
        {
            int random = UnityEngine.Random.Range(0, int.MaxValue);
            string s = $"TEXT_{random}";
            if (!IDs.ContainsKey(text))
            {
                IDs.Add(text, new List<string>());
            }
            IDs[text].Add(s);
            return s;
        }
        internal static void BeginIEnumerator(string name, TMP_Text text, IEnumerator enumerator)
        {
            BeginCoroutine(enumerator, name, text, dic_TextAnimator);
        }
        internal static void Delay(string name, TMP_Text text, Action action, float interval)
        {
            BeginCoroutine(MCoroutineUtility.Delay(action, interval), name, text, dic_TextAnimator);
        }
        internal static void Repeat(string name, TMP_Text text, Action action, bool startDo, int count, float interval, Action onFinish = null)
        {
            BeginCoroutine(MCoroutineUtility.Repeat(action, startDo, count, interval, onFinish), name, text, dic_TextAnimator);
        }
        internal static void Loop(string name, TMP_Text text, Action action, float startInterval, float repeatInterval)
        {
            BeginCoroutine(MCoroutineUtility.Loop(action, startInterval, repeatInterval), name, text, dic_TextAnimator);
        }
        internal static void Tween01(string name, TMP_Text text, Action<float> action, MCurve curve, float duration, Action onFinish = null)
        {
            BeginCoroutine(MCoroutineManager.Instance.UnsacledFixedTweenRoutine(action, curve, duration, 0, 1, onFinish), name, text, dic_TextAnimator);
        }

        internal static void StopAllCoroutines(TMP_Text text)
        {
            if (IDs.ContainsKey(text))
            {
                foreach (string s in IDs[text])
                {
                    EndCoroutine(s, dic_TextAnimator);
                }
                IDs[text].Clear();
            }
        }
        internal static void StopCoroutines(TMP_Text text, string id)
        {
            if (IDs.ContainsKey(text))
            {
                if (dic_TextAnimator.ContainsKey(id))
                {
                    EndCoroutine(id, dic_TextAnimator);
                    IDs[text].Remove(id);
                }
            }
        }

        private static void BeginCoroutine(IEnumerator fun, string name, TMP_Text text, Dictionary<string, Coroutine> dic, Action onFinish = null)
        {
            MCoroutineManager.Instance.StartCoroutine(BeginCoroutinueInternal(fun, name, text, dic, onFinish));
        }

        private static IEnumerator BeginCoroutinueInternal(IEnumerator enumerator, string name, TMP_Text text, Dictionary<string, Coroutine> dic, Action onFinish)
        {
            Coroutine coroutine = MCoroutineManager.Instance.StartCoroutine(enumerator);
            dic.Add(name, coroutine);

            yield return coroutine;

            OnCoroutineFinished(name, text, dic);
            onFinish?.Invoke();

            yield break;
        }
        private static void OnCoroutineFinished(string name, TMP_Text text, Dictionary<string, Coroutine> dic)
        {
            if (dic.ContainsKey(name))
            {
                dic.Remove(name);
                if (IDs.ContainsKey(text))
                {
                    IDs[text].Remove(name);
                }
            }
        }

        private static bool EndCoroutine(string name, Dictionary<string, Coroutine> dic)
        {
            if (dic.ContainsKey(name))
            {
                MCoroutineManager.Instance.EndCoroutine(dic[name]);
                dic.Remove(name);
                return true;
            }
            //时间到了情况---会自动从dic中删除，无需再次停止
            return false;
        }
    }
}