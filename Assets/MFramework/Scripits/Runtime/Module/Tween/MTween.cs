using System;
using UnityEngine;

namespace MFramework
{
    public static class MTween
    {
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
        public static void Scale(this Transform t, float scaleMultipler, MCurve curve, float duration, Action onFinish = null)
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
        public static void SinScale(this Transform t, MCurve curve, float scaleMultipler = 1, float frequency = 1, float duration = 1, Action onFinish = null)
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
        public static void SinScaleLoop(this Transform t, MCurve curve, float scaleMultipler = 1, float frequency = 1)
        {
            Vector3 oScale = t.localScale;
            DoTween01NoRecord((f) =>
            {
                //y=0.5sin(2πx)+1
                float y = 0.5f * scaleMultipler * Mathf.Sin(frequency * 2 * Mathf.PI * f) + 1;
                t.localScale = oScale * y;
            }, curve, 1, () => 
            {
                t.SinScaleLoop(curve, scaleMultipler, frequency);
            });
        }
        #endregion
    }
}