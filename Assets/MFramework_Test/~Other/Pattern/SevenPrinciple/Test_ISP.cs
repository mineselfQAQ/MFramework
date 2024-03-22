using MFramework;
using UnityEngine;

public class Test_ISP : MonoBehaviour
{
    //private void Start()
    //{
    //    IWorker worker = new Worker();
    //    worker.Eat();
    //    worker.Rest();
    //    worker.Work();
    //    //此时如果出现Student，IWorker的泛用性就消失了
    //}

    //public interface IWorker
    //{
    //    void Work();
    //    void Eat();
    //    void Rest();
    //}

    //public class Worker : IWorker
    //{
    //    public void Eat()
    //    {
    //        MLog.Print("Eat");
    //    }

    //    public void Rest()
    //    {
    //        MLog.Print("Rest");
    //    }

    //    public void Work()
    //    {
    //        MLog.Print("Work");
    //    }
    //}

    private void Start()
    {
        IWorkable worker = new Worker();
        worker.Work();

        IEatable eater = new Worker();
        eater.Eat();

        IRestable rester = new Worker();
        rester.Rest();
    }

    public interface IWorkable
    {
        void Work();
    }

    public interface IEatable
    {
        void Eat();
    }

    public interface IRestable
    {
        void Rest();
    }

    public class Worker : IWorkable, IEatable, IRestable
    {
        public void Eat()
        {
            MLog.Print("Eat");
        }

        public void Rest()
        {
            MLog.Print("Rest");
        }

        public void Work()
        {
            MLog.Print("Work");
        }
    }
}
