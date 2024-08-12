using System;
using TMPro;

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
    }
}