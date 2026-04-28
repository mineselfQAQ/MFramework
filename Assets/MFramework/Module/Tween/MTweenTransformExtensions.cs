using System;
using UnityEngine;

namespace MFramework.Tween
{
    public static class MTweenTransformExtensions
    {
        public static Coroutine ScaleNoRecord(this MTweenManager tween, Transform transform, float scaleMultiplier, MCurve curve, float duration, Action onFinish = null)
        {
            Vector3 sourceScale = transform.localScale;
            Vector3 targetScale = sourceScale * scaleMultiplier;

            return tween.DoTween01NoRecord(value =>
            {
                transform.localScale = Vector3.Lerp(sourceScale, targetScale, value);
            }, curve, duration, onFinish);
        }

        public static Coroutine SinScaleNoRecord(this MTweenManager tween, Transform transform, MCurve curve, float scaleMultiplier = 1f, float frequency = 1f, float duration = 1f, Action onFinish = null)
        {
            Vector3 originalScale = transform.localScale;

            return tween.DoTweenNoRecord(value =>
            {
                float y = 0.5f * scaleMultiplier * Mathf.Sin(frequency * 2f * Mathf.PI * value) + 1f;
                transform.localScale = originalScale * y;
            }, curve, duration, 0f, duration, onFinish);
        }

        public static Coroutine SinScaleLoopNoRecord(this MTweenManager tween, Transform transform, MCurve curve, float scaleMultiplier = 1f, float frequency = 1f)
        {
            Vector3 originalScale = transform.localScale;

            return tween.DoTween01NoRecord(value =>
            {
                float y = 0.5f * scaleMultiplier * Mathf.Sin(frequency * 2f * Mathf.PI * value) + 1f;
                transform.localScale = originalScale * y;
            }, curve, 1f, () => tween.SinScaleLoopNoRecord(transform, curve, scaleMultiplier, frequency));
        }

        public static Coroutine SinLoopNoRecord(this MTweenManager tween, Action<float> action, MCurve curve, float scaleMultiplier = 1f, float frequency = 1f, bool random = false)
        {
            float from = 0f;
            float to = 1f;

            if (random)
            {
                from = UnityEngine.Random.Range(0f, 1f);
                to = from + 1f;
            }

            return tween.DoTweenNoRecord(value =>
            {
                float y = 0.5f * scaleMultiplier * Mathf.Sin(2f * Mathf.PI * value);
                action(y);
            }, curve, 1f / frequency, from, to, () => tween.SinLoopNoRecord(action, curve, scaleMultiplier, frequency));
        }

        public static Coroutine SinLoop(this MTweenManager tween, string name, Action<float> action, MCurve curve, float scaleMultiplier = 1f, float frequency = 1f, bool random = false)
        {
            float from = random ? UnityEngine.Random.Range(0f, 1f) : 0f;
            int to = int.MaxValue;

            return tween.DoTween(name, value =>
            {
                float y = 0.5f * scaleMultiplier * Mathf.Sin(frequency * 2f * Mathf.PI * value);
                action(y);
            }, curve, to - from, from, to);
        }
    }
}
