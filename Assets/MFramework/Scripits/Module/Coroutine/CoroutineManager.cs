using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    public class CoroutineManager : MonoSingleton<CoroutineManager>
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

        private CoroutineManager()
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

        public void BeginCoroutine(IEnumerator fun, string name)
        {
            StartCoroutine(BeginCoroutinueInternal(fun, name));
        }

        public bool EndCoroutine(string name)
        {
            if (!dic.ContainsKey(name))
            {
                MLog.Print($"字典中没有名为{name}的Coroutine，请检查名字是否正确或协程是否已结束", MLogType.Warning);
                return false;
            }

            StopCoroutine(dic[name]);
            dic.Remove(name);
            count--;

            return true;
        }

        public void EndAllCoroutine()
        {
            foreach (var value in dic.Values)
            {
                StopCoroutine(value);
            }
            dic.Clear();
            count = 0;
        }

        internal void BeginCoroutineAndNotRecord(IEnumerator enumerator)
        {
            StartCoroutine(enumerator);
        }

        private IEnumerator BeginCoroutinueInternal(IEnumerator enumerator, string name)
        {
            Coroutine coroutine = StartCoroutine(enumerator);
            dic.Add(name, coroutine);
            count++;

            yield return coroutine;

            OnCoroutineFinished(name);

            yield break;
        }

        private void OnCoroutineFinished(string name)
        {
            if (dic.ContainsKey(name))
            {
                dic.Remove(name);
                count--;
            }
        }
    }
}