using System.Reflection;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// Awake()/Start()增强版，可控执行顺序
    /// <para>***注意***仅在需要延迟时使用，避免浪费性能</para>
    /// </summary>
    public abstract class MonoBehaviourPls : MonoBehaviour
    {
        protected virtual void Awake00() { }
        protected virtual void Awake01() { }
        protected virtual void Awake02() { }
        protected virtual void Awake03() { }
        protected virtual void Awake04() { }

        protected virtual void Start00() { }
        protected virtual void Start01() { }
        protected virtual void Start02() { }
        protected virtual void Start03() { }
        protected virtual void Start04() { }

        protected void Awake()
        {
            Awake00();
            TryInvokeWithDelay("Awake01", 1);
            TryInvokeWithDelay("Awake02", 2);
            TryInvokeWithDelay("Awake03", 3);
            TryInvokeWithDelay("Awake04", 4);
        }

        protected void Start()
        {
            Start00();
            TryInvokeWithDelay("Start01", 1);
            TryInvokeWithDelay("Start02", 2);
            TryInvokeWithDelay("Start03", 3);
            TryInvokeWithDelay("Start04", 4);
        }

        private void TryInvokeWithDelay(string methodName, int delayFrame)
        {
            MethodInfo method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

            //避免没有重写的函数调用
            if (method != null && method.DeclaringType != typeof(MonoBehaviourPls))
            {
                MCoroutineManager.Instance.DelayFrame(() => method.Invoke(this, null), delayFrame);
            }
        }
    }
}
