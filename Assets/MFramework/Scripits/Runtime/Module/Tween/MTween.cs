using System;
using UnityEngine;

namespace MFramework
{
    public static class MTween
    {
        public static void UnscaledFixedDoTweenNoRecord(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish = null)
        {
            MCoroutineManager.Instance.UnscaledFixedTweenNoRecord(action, curve, duration, startValue, endValue, onFinish);
        }
        public static void UnscaledFixedDoTween01NoRecord(Action<float> action, MCurve curve, float duration, Action onFinish = null)
        {
            MCoroutineManager.Instance.UnscaledFixedTweenNoRecord(action, curve, duration, 0, 1, onFinish);
        }
        public static void UnscaledFixedDoTween(string name, Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish = null)
        {
            MCoroutineManager.Instance.UnscaledFixedTween(name, action, curve, duration, startValue, endValue, onFinish);
        }
        public static void UnscaledFixedDoTween01(string name, Action<float> action, MCurve curve, float duration, Action onFinish = null)
        {
            MCoroutineManager.Instance.UnscaledFixedTween(name, action, curve, duration, 0, 1, onFinish);
        }

        public static void FixedDoTweenNoRecord(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish = null)
        {
            MCoroutineManager.Instance.FixedTweenNoRecord(action, curve, duration, startValue, endValue, onFinish);
        }
        public static void FixedDoTween01NoRecord(Action<float> action, MCurve curve, float duration, Action onFinish = null)
        {
            MCoroutineManager.Instance.FixedTweenNoRecord(action, curve, duration, 0, 1, onFinish);
        }
        public static void FixedDoTween(string name, Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish = null)
        {
            MCoroutineManager.Instance.FixedTween(name, action, curve, duration, startValue, endValue, onFinish);
        }
        public static void FixedDoTween01(string name, Action<float> action, MCurve curve, float duration, Action onFinish = null)
        {
            MCoroutineManager.Instance.FixedTween(name, action, curve, duration, 0, 1, onFinish);
        }

        public static void UnscaledDoTweenNoRecord(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish = null)
        {
            MCoroutineManager.Instance.UnscaledTweenNoRecord(action, curve, duration, startValue, endValue, onFinish);
        }
        public static void UnscaledDoTween01NoRecord(Action<float> action, MCurve curve, float duration, Action onFinish = null)
        {
            MCoroutineManager.Instance.UnscaledTweenNoRecord(action, curve, duration, 0, 1, onFinish);
        }
        public static void UnscaledDoTween(string name, Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish = null)
        {
            MCoroutineManager.Instance.UnsacledTween(name, action, curve, duration, startValue, endValue, onFinish);
        }
        public static void UnscaledDoTween01(string name, Action<float> action, MCurve curve, float duration, Action onFinish = null)
        {
            MCoroutineManager.Instance.UnsacledTween(name, action, curve, duration, 0, 1, onFinish);
        }

        public static void DoTweenNoRecord(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish = null)
        {
            MCoroutineManager.Instance.TweenNoRecord(action, curve, duration, startValue, endValue, onFinish);
        }
        public static void DoTween01NoRecord(Action<float> action, MCurve curve, float duration, Action onFinish = null)
        {
            MCoroutineManager.Instance.TweenNoRecord(action, curve, duration, 0, 1, onFinish);
        }
        public static void DoTween(string name, Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish = null)
        {
            MCoroutineManager.Instance.Tween(name, action, curve, duration, startValue, endValue, onFinish);
        }
        public static void DoTween01(string name, Action<float> action, MCurve curve, float duration, Action onFinish = null)
        {
            MCoroutineManager.Instance.Tween(name, action, curve, duration, 0, 1, onFinish);
        }

        #region 预制操作
        public static void ScaleNoRecord(this Transform t, float scaleMultipler, MCurve curve, float duration, Action onFinish = null)
        {
            Vector3 srcScale = t.localScale;
            Vector3 desScale = srcScale * scaleMultipler;
            DoTween01NoRecord((f) =>
            {
                t.localScale = Vector3.Lerp(srcScale, desScale, f);
            }, curve, duration, onFinish);
        }

        /// <param name="scaleMultipler">振幅，默认区间[0.5,1.5]，公式[1-0.5*scale,1+0.5*scale]</param>
        /// <param name="frequency">频率，默认1秒1次</param>
        public static void SinScaleNoRecord(this Transform t, MCurve curve, float scaleMultipler = 1, float frequency = 1, float duration = 1, Action onFinish = null)
        {
            Vector3 oScale = t.localScale;
            DoTweenNoRecord((f) =>
            {
                //y=0.5sin(2πx)+1
                float y = 0.5f * scaleMultipler * Mathf.Sin(frequency * 2 * Mathf.PI * f) + 1;
                t.localScale = oScale * y;
            }, curve, duration, 0, duration, onFinish);
        }

        /// <param name="scaleMultipler">振幅，默认区间[0.5,1.5]，公式[1-0.5*scale,1+0.5*scale]</param>
        /// <param name="frequency">频率，默认1秒1次</param>
        public static void SinScaleLoopNoRecord(this Transform t, MCurve curve, float scaleMultipler = 1, float frequency = 1)
        {
            Vector3 oScale = t.localScale;
            DoTween01NoRecord((f) =>
            {
                //y=0.5sin(2πx)+1
                float y = 0.5f * scaleMultipler * Mathf.Sin(frequency * 2 * Mathf.PI * f) + 1;
                t.localScale = oScale * y;
            }, curve, 1, () => 
            {
                t.SinScaleLoopNoRecord(curve, scaleMultipler, frequency);
            });
        }

        public static void SinLoopNoRecord(Action<float> action, MCurve curve, float scaleMultipler = 1, float frequency = 1, bool random = false)
        {
            float from = 0, to = 1;
            if (random)
            {
                from = UnityEngine.Random.Range(0f, 1f);
                to = from + 1;
            }

            DoTweenNoRecord((f) =>
            {
                //y=0.5sin(2πx)
                float y = 0.5f * scaleMultipler * Mathf.Sin(2 * Mathf.PI * f);
                action.Invoke(y);
            }, curve, 1 / frequency, from, to, () =>
            {
                SinLoopNoRecord(action, curve, scaleMultipler, frequency);
            });
        }

        //TODO:有一定时间限制，为68年，主要问题为精度问题
        public static void SinLoop(string name, Action<float> action, MCurve curve, float scaleMultipler = 1, float frequency = 1, bool random = false)
        {
            float from = 0;
            int to = int.MaxValue;
            if (random)
            {
                from = UnityEngine.Random.Range(0f, 1f);
            }

            DoTween(name, (f) =>
            {
                //y=0.5sin(2πx)
                float y = 0.5f * scaleMultipler * Mathf.Sin(frequency * 2 * Mathf.PI * f);
                action.Invoke(y);
            }, curve, to - from, from, to);
        }
        #endregion
    }
}