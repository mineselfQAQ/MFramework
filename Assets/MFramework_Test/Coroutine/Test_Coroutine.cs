using MFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Coroutine : MonoBehaviour
{
    private void Start()
    {
        TestCoroutineManager.Instance.StartCoroutine(Countdown());
    }
    private void Update()
    {
        TestCoroutineManager.Instance.Update(Time.deltaTime);
    }
    IEnumerator Countdown()
    {
        MLog.Print(3);
        yield return new MWaitForSeconds(1);
        MLog.Print(2);
        yield return new MWaitForSeconds(1);
        MLog.Print(1);
        yield return new MWaitForSeconds(1);
        MLog.Print(0);
    }

    public class TestCoroutineManager : Singleton<TestCoroutineManager>
    {
        List<IEnumerator> enumerators = new List<IEnumerator>();

        private TestCoroutineManager()
        {

        }

        public void StartCoroutine(IEnumerator enumerator)
        {
            enumerators.Add(enumerator);
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < enumerators.Count; i++)
            {
                IEnumerator enumerator = enumerators[i];

                //核心：当yield return的是MWaitForSeconds的时候
                if (enumerator.Current is MWaitForSeconds waitForSeconds)
                {
                    //将MWaitForSeconds.Duration的值减少(倒计时)
                    waitForSeconds.Duration -= deltaTime;
                    //如果还没到0，说明还没完成，不要MoveNext()直接continue，
                    //那么就会在下一帧继续执行这块内容
                    if (waitForSeconds.Duration > 0)
                        continue;
                }

                //对于其它任意情况(如yield return 0;)，
                //由于没有编写特殊逻辑，所以发生的事就是MoveNext()，同时也就在yield处暂停了一帧

                //该IEnumerator结束，移除
                if (!enumerator.MoveNext())
                {
                    enumerators.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    public class MWaitForSeconds
    {
        public float Duration { get; set; }

        public MWaitForSeconds(float duration)
        {
            Duration = duration;
        }
    }
}
