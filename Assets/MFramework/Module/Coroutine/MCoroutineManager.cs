using System;
using System.Collections;
using System.Collections.Generic;
using MFramework.Core;
using UnityEngine;

namespace MFramework.Coroutines
{
    public class MCoroutineManager
    {
        private const string RUNNER_NAME = "#MCoroutineRunner#";

        private readonly Dictionary<string, Coroutine> _namedCoroutines = new Dictionary<string, Coroutine>();
        private readonly ILog _log;
        private CoroutineRunner _runner;

        public MCoroutineManager(ILog log = null)
        {
            _log = log ?? new UserLog(nameof(MCoroutineManager));
        }

        public int Count => _namedCoroutines.Count;

        public Coroutine BeginCoroutineNoRecord(Action action)
        {
            return StartCoroutineNoRecord(MCoroutineUtil.Do(action));
        }

        public Coroutine BeginCoroutineWithCallBackNoRecord(Action action, Action onFinish)
        {
            return StartCoroutineNoRecord(MCoroutineUtil.Do(action), onFinish);
        }

        public Coroutine StartCoroutineNoRecord(IEnumerator routine, Action onFinish = null)
        {
            if (routine == null)
            {
                throw new ArgumentNullException(nameof(routine));
            }

            IEnumerator finalRoutine = onFinish == null
                ? routine
                : RunCoroutineWithFinish(routine, onFinish);

            return GetRunner().StartCoroutine(finalRoutine);
        }

        public void EndCoroutine(Coroutine coroutine)
        {
            if (coroutine == null) return;

            GetRunner().StopCoroutine(coroutine);
        }

        public Coroutine StartCoroutine(IEnumerator routine, string name, Action onFinish = null)
        {
            if (routine == null)
            {
                throw new ArgumentNullException(nameof(routine));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Coroutine name cannot be null or empty.", nameof(name));
            }

            if (_namedCoroutines.ContainsKey(name))
            {
                _log.W($"{nameof(MCoroutineManager)}: coroutine '{name}' already exists.");
                return null;
            }

            Coroutine coroutine = GetRunner().StartCoroutine(RunNamedCoroutine(routine, name, onFinish));
            _namedCoroutines.Add(name, coroutine);
            return coroutine;
        }

        public bool EndCoroutine(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            if (!_namedCoroutines.TryGetValue(name, out Coroutine coroutine)) return false;

            GetRunner().StopCoroutine(coroutine);
            _namedCoroutines.Remove(name);
            return true;
        }

        public void EndAllCoroutines()
        {
            if (_runner != null)
            {
                foreach (Coroutine coroutine in _namedCoroutines.Values)
                {
                    if (coroutine != null)
                    {
                        _runner.StopCoroutine(coroutine);
                    }
                }
            }

            _namedCoroutines.Clear();
        }

        public Coroutine DelayFrame(Action action, int frame)
        {
            return StartCoroutineNoRecord(MCoroutineUtil.DelayFrame(action, frame));
        }

        public Coroutine DelayOneFrame(Action action)
        {
            return DelayFrame(action, 1);
        }

        public Coroutine DelayWithTimeScaleNoRecord(Action action, float interval)
        {
            return StartCoroutineNoRecord(MCoroutineUtil.DelayWithTimeScale(action, interval));
        }

        public Coroutine DelayNoRecord(Action action, float interval)
        {
            return StartCoroutineNoRecord(MCoroutineUtil.Delay(action, interval));
        }

        public Coroutine RepeatNoRecord(Action action, bool startDo, int count, float interval, Action onFinish = null)
        {
            return StartCoroutineNoRecord(MCoroutineUtil.Repeat(action, startDo, count, interval), onFinish);
        }

        public Coroutine DelayRepeatNoRecord(Action action, float startInterval, int repeatCount, float repeatInterval, Action onFinish = null)
        {
            return StartCoroutineNoRecord(MCoroutineUtil.DelayRepeat(action, startInterval, repeatCount, repeatInterval), onFinish);
        }

        public Coroutine LoopNoRecord(Action action, float startInterval, float repeatInterval)
        {
            return StartCoroutineNoRecord(MCoroutineUtil.Loop(action, startInterval, repeatInterval));
        }

        public Coroutine WaitNoRecord(Action onFinish, BoolWrapper flag, int moreFrame = 0)
        {
            return StartCoroutineNoRecord(MCoroutineUtil.Wait(onFinish, flag, moreFrame));
        }

        public Coroutine Delay(string name, Action action, float interval)
        {
            return StartCoroutine(MCoroutineUtil.Delay(action, interval), name);
        }

        public Coroutine Repeat(string name, Action action, bool startDo, int count, float interval, Action onFinish = null)
        {
            return StartCoroutine(MCoroutineUtil.Repeat(action, startDo, count, interval), name, onFinish);
        }

        public Coroutine DelayRepeat(string name, Action action, float startInterval, int repeatCount, float repeatInterval, Action onFinish = null)
        {
            return StartCoroutine(MCoroutineUtil.DelayRepeat(action, startInterval, repeatCount, repeatInterval), name, onFinish);
        }

        public Coroutine Loop(string name, Action action, float startInterval, float repeatInterval)
        {
            return StartCoroutine(MCoroutineUtil.Loop(action, startInterval, repeatInterval), name);
        }

        public Coroutine Wait(string name, Action onFinish, BoolWrapper flag, int moreFrame = 0)
        {
            return StartCoroutine(MCoroutineUtil.Wait(onFinish, flag, moreFrame), name);
        }

        public void EnsureRunner()
        {
            GetRunner();
        }

        private CoroutineRunner GetRunner()
        {
            if (_runner != null) return _runner;

            GameObject runnerObject = new GameObject(RUNNER_NAME)
            {
                hideFlags = HideFlags.NotEditable,
            };
            if (Application.isPlaying)
            {
                UnityEngine.Object.DontDestroyOnLoad(runnerObject);
            }

            _runner = runnerObject.AddComponent<CoroutineRunner>();
            return _runner;
        }

        public void Shutdown()
        {
            EndAllCoroutines();

            if (_runner != null)
            {
                DestroyRunner(_runner.gameObject);
                _runner = null;
            }
        }

        private IEnumerator RunNamedCoroutine(IEnumerator routine, string name, Action onFinish)
        {
            yield return routine;

            _namedCoroutines.Remove(name);
            onFinish?.Invoke();
        }

        private IEnumerator RunCoroutineWithFinish(IEnumerator routine, Action onFinish)
        {
            yield return routine;
            onFinish?.Invoke();
        }

        private sealed class CoroutineRunner : MonoBehaviour
        {
        }

        private static void DestroyRunner(GameObject runnerObject)
        {
            if (runnerObject == null) return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEngine.Object.DestroyImmediate(runnerObject);
                return;
            }
#endif

            UnityEngine.Object.Destroy(runnerObject);
        }
    }
}
