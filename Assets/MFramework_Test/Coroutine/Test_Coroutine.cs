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

                //���ģ���yield return����MWaitForSeconds��ʱ��
                if (enumerator.Current is MWaitForSeconds waitForSeconds)
                {
                    //��MWaitForSeconds.Duration��ֵ����(����ʱ)
                    waitForSeconds.Duration -= deltaTime;
                    //�����û��0��˵����û��ɣ���ҪMoveNext()ֱ��continue��
                    //��ô�ͻ�����һ֡����ִ���������
                    if (waitForSeconds.Duration > 0)
                        continue;
                }

                //���������������(��yield return 0;)��
                //����û�б�д�����߼������Է������¾���MoveNext()��ͬʱҲ����yield����ͣ��һ֡

                //��IEnumerator�������Ƴ�
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
