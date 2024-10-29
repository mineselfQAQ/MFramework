using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MFramework
{
    public class MainThreadSynchronizationContext : SynchronizationContext
    {
        private static MainThreadSynchronizationContext instance;
        public static MainThreadSynchronizationContext Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainThreadSynchronizationContext();
                }

                return instance;
            }
        }

        private int mMainThreadId = Thread.CurrentThread.ManagedThreadId;
        private readonly ConcurrentQueue<Action> mConcurrentQueue = new ConcurrentQueue<Action>();
        private Action mTempAction;

        //ע�⣡����
        //�����Ҫʹ���·���Post()/Send()����ô���������̵߳�Update()�е��ø÷���
        //��Ȼ������������
        public void Update()
        {
            int count = mConcurrentQueue.Count;
            for (int i = 0; i < count; ++i)
            {
                if (mConcurrentQueue.TryDequeue(out mTempAction))
                {
                    mTempAction();
                }
            }
        }

        //�첽��Ϣ
        public override void Post(SendOrPostCallback sendOrPostCallback, object state = null)
        {
            //��ǰ�߳�Ϊ���̵߳Ļ�ֱ��ִ�м���
            if (Thread.CurrentThread.ManagedThreadId == mMainThreadId)
            {
                sendOrPostCallback(state);
                return;
            }

            //ֻҪ��ǰ�̲߳������̣߳��ͻὫ�ص��ŵ������У������߳��е�Update()���ò�ִ��
            mConcurrentQueue.Enqueue(() => { sendOrPostCallback(state); });
        }
        //ͬ����Ϣ
        public override void Send(SendOrPostCallback sendOrPostCallback, object state = null)
        {
            if (Thread.CurrentThread.ManagedThreadId == mMainThreadId)
            {
                ThreadPool.QueueUserWorkItem(sendOrPostCallback.Invoke, state);
                return;
            }

            mConcurrentQueue.Enqueue(() => { sendOrPostCallback(state); });
        }
    }
}