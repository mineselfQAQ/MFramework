using MFramework;
using MFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    private void Start()
    {
        //Test01();
        Test02();
    }

    private void Update()
    {

    }

    public void Test01()
    {
        int count = 0;
        var demo = new Demo(() => count++);
        Debug.Log(count);
        demo.DoJob();
        Debug.Log(count);
    }

    public void Test02()
    {
        var cnt = CreateCounter();
        cnt.Invoke();
        cnt.Invoke();
        cnt.Invoke();

        Action CreateCounter()
        {
            int count = 0;
            return () => Debug.Log(++count);
        }
    }

    public void Error1()
    {
        var actions = CreateActions();
        actions[0].Invoke(10);
        actions[1].Invoke(10);
        actions[2].Invoke(10);

        Action<int>[] CreateActions(int count = 3)
        {
            var actions = new Action<int>[count];
            for (int i = 0; i < count; i++)
            {
                //����棺
                //��Ϊ�ڵ���CreateActions()ʱ���������Ѿ�ִ������ˣ�����ζ��i�Ѿ������񣬵����յ�3
                //actions[i] = x => Debug.Log(x * i);

                //��ȷ��
                int j = i;
                actions[i] = x => Debug.Log(x * j);
            }
            return actions;
        }
    }

    public void Error2()
    {
        int[] arr = new int[] { 1, 3, 5 };

        var actions = new Action[arr.Length];
        for (int i = 0; i < arr.Length; i++)
        {
            //�����
            //actions[i] = () => Debug.Log(arr[i]);

            //��ȷ��
            int j = i;
            actions[i] = () => Debug.Log(arr[j]);
        }

        foreach (var action in actions)
        {
            action();
        }
    }

    class Demo
    {
        private readonly Action _callback;

        public Demo(Action callback)
        {
            _callback = callback;
        }

        public void DoJob()
        {
            Debug.Log("Job Done.");
            _callback.Invoke();
        }
    }
}