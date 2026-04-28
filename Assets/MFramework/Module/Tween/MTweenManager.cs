using System;
using System.Collections;
using MFramework.Coroutines;
using UnityEngine;

namespace MFramework.Tween
{
    public class MTweenManager
    {
        private readonly MCoroutineManager _coroutineManager;

        public MTweenManager(MCoroutineManager coroutineManager)
        {
            _coroutineManager = coroutineManager ?? throw new ArgumentNullException(nameof(coroutineManager));
        }

        public Coroutine UnscaledFixedDoTweenNoRecord(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish = null)
        {
            return _coroutineManager.StartCoroutineNoRecord(UnscaledFixedTweenRoutine(action, curve, duration, startValue, endValue), onFinish);
        }

        public Coroutine UnscaledFixedDoTween01NoRecord(Action<float> action, MCurve curve, float duration, Action onFinish = null)
        {
            return UnscaledFixedDoTweenNoRecord(action, curve, duration, 0f, 1f, onFinish);
        }

        public Coroutine UnscaledFixedDoTween(string name, Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish = null)
        {
            return _coroutineManager.StartCoroutine(UnscaledFixedTweenRoutine(action, curve, duration, startValue, endValue), name, onFinish);
        }

        public Coroutine UnscaledFixedDoTween01(string name, Action<float> action, MCurve curve, float duration, Action onFinish = null)
        {
            return UnscaledFixedDoTween(name, action, curve, duration, 0f, 1f, onFinish);
        }

        public Coroutine FixedDoTweenNoRecord(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish = null)
        {
            return _coroutineManager.StartCoroutineNoRecord(FixedTweenRoutine(action, curve, duration, startValue, endValue), onFinish);
        }

        public Coroutine FixedDoTween01NoRecord(Action<float> action, MCurve curve, float duration, Action onFinish = null)
        {
            return FixedDoTweenNoRecord(action, curve, duration, 0f, 1f, onFinish);
        }

        public Coroutine FixedDoTween(string name, Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish = null)
        {
            return _coroutineManager.StartCoroutine(FixedTweenRoutine(action, curve, duration, startValue, endValue), name, onFinish);
        }

        public Coroutine FixedDoTween01(string name, Action<float> action, MCurve curve, float duration, Action onFinish = null)
        {
            return FixedDoTween(name, action, curve, duration, 0f, 1f, onFinish);
        }

        public Coroutine UnscaledDoTweenNoRecord(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish = null)
        {
            return _coroutineManager.StartCoroutineNoRecord(UnscaledTweenRoutine(action, curve, duration, startValue, endValue), onFinish);
        }

        public Coroutine UnscaledDoTween01NoRecord(Action<float> action, MCurve curve, float duration, Action onFinish = null)
        {
            return UnscaledDoTweenNoRecord(action, curve, duration, 0f, 1f, onFinish);
        }

        public Coroutine UnscaledDoTween(string name, Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish = null)
        {
            return _coroutineManager.StartCoroutine(UnscaledTweenRoutine(action, curve, duration, startValue, endValue), name, onFinish);
        }

        public Coroutine UnscaledDoTween01(string name, Action<float> action, MCurve curve, float duration, Action onFinish = null)
        {
            return UnscaledDoTween(name, action, curve, duration, 0f, 1f, onFinish);
        }

        public Coroutine DoTweenNoRecord(Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish = null)
        {
            return _coroutineManager.StartCoroutineNoRecord(TweenRoutine(action, curve, duration, startValue, endValue), onFinish);
        }

        public Coroutine DoTween01NoRecord(Action<float> action, MCurve curve, float duration, Action onFinish = null)
        {
            return DoTweenNoRecord(action, curve, duration, 0f, 1f, onFinish);
        }

        public Coroutine DoTween(string name, Action<float> action, MCurve curve, float duration, float startValue, float endValue, Action onFinish = null)
        {
            return _coroutineManager.StartCoroutine(TweenRoutine(action, curve, duration, startValue, endValue), name, onFinish);
        }

        public Coroutine DoTween01(string name, Action<float> action, MCurve curve, float duration, Action onFinish = null)
        {
            return DoTween(name, action, curve, duration, 0f, 1f, onFinish);
        }

        public bool Stop(string name)
        {
            return _coroutineManager.EndCoroutine(name);
        }

        private static IEnumerator UnscaledFixedTweenRoutine(Action<float> action, MCurve curve, float duration, float startValue, float endValue)
        {
            ValidateTween(action, curve, duration);

            float step = Mathf.Max(1f, duration / Time.fixedUnscaledDeltaTime);
            float length = endValue - startValue;

            for (int i = 0; i < step; i++)
            {
                float value = startValue + MCurveSampler.Sample(curve, i / step) * length;
                action(value);
                yield return new WaitForSecondsRealtime(Time.fixedUnscaledDeltaTime);
            }

            action(GetFinalValue(curve, startValue, endValue));
        }

        private static IEnumerator FixedTweenRoutine(Action<float> action, MCurve curve, float duration, float startValue, float endValue)
        {
            ValidateTween(action, curve, duration);

            float step = Mathf.Max(1f, duration / Time.fixedDeltaTime);
            float length = endValue - startValue;

            for (int i = 0; i < step; i++)
            {
                float value = startValue + MCurveSampler.Sample(curve, i / step) * length;
                action(value);
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            action(GetFinalValue(curve, startValue, endValue));
        }

        private static IEnumerator UnscaledTweenRoutine(Action<float> action, MCurve curve, float duration, float startValue, float endValue)
        {
            ValidateTween(action, curve, duration);

            float elapsed = 0f;
            float length = endValue - startValue;

            while (elapsed < duration)
            {
                float progress = Mathf.Clamp01(elapsed / duration);
                float value = startValue + MCurveSampler.Sample(curve, progress) * length;
                action(value);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            action(GetFinalValue(curve, startValue, endValue));
        }

        private static IEnumerator TweenRoutine(Action<float> action, MCurve curve, float duration, float startValue, float endValue)
        {
            ValidateTween(action, curve, duration);

            float elapsed = 0f;
            float length = endValue - startValue;

            while (elapsed < duration)
            {
                float progress = Mathf.Clamp01(elapsed / duration);
                float value = startValue + MCurveSampler.Sample(curve, progress) * length;
                action(value);
                elapsed += Time.deltaTime;
                yield return null;
            }

            action(GetFinalValue(curve, startValue, endValue));
        }

        private static void ValidateTween(Action<float> action, MCurve curve, float duration)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (curve == null)
            {
                throw new ArgumentNullException(nameof(curve));
            }

            if (duration <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(duration));
            }
        }

        private static float GetFinalValue(MCurve curve, float startValue, float endValue)
        {
            return curve.CurveDir == CurveDir.Increment ? endValue : startValue;
        }
    }
}
