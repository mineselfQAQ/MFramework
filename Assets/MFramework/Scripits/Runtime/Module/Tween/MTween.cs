using System;
using TMPro;

namespace MFramework
{
    public static class MTween
    {
        public static void DoTween(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action<float> onFinish = null)
        {
            MCoroutineManager.Instance.TweenNoRecord(action, curve, duration, startValue, endValue, onFinish);
        }

        public static void DoTween01(Action<float> action, MCurve curve, float duration, Action<float> onFinish = null)
        {
            MCoroutineManager.Instance.TweenNoRecord(action, curve, duration, 0, 1, onFinish);
        }
    }
}