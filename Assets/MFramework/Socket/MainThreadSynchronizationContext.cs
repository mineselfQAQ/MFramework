using System;
using System.Collections.Concurrent;
using System.Threading;

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

    //注意！！！
    //如果想要使用下方的Post()/Send()，那么必须在主线程的Update()中调用该方法
    //不然输出都不会输出
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

    //异步消息
    public override void Post(SendOrPostCallback sendOrPostCallback, object state = null)
    {
        //当前线程为主线程的话直接执行即可
        if (Thread.CurrentThread.ManagedThreadId == mMainThreadId)
        {
            sendOrPostCallback(state);
            return;
        }

        //只要当前线程不是主线程，就会将回调放到队列中，待主线程中的Update()调用并执行
        mConcurrentQueue.Enqueue(() => { sendOrPostCallback(state); });
    }
    //同步消息
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