using MFramework;
using System.Collections.Generic;
using UnityEngine;

public class Test_CARP : MonoBehaviour
{
    //private void Start()
    //{
    //    //如果这只是一个Student还好，但是如果它去打工还是一个Employee就不行了
    //    Student student = new Student();
    //    student.DoStudentWork();
    //}

    //public class Person
    //{

    //}

    //public class Employee : Person
    //{
    //    public void DoEmployeeWork()
    //    {
    //        MLog.Print("DoEmployeeWork");
    //    }
    //}

    //public class Manager : Person
    //{
    //    public void DoManagerWork()
    //    {
    //        MLog.Print("DoManagerWork");
    //    }
    //}

    //public class Student : Person
    //{
    //    public void DoStudentWork()
    //    {
    //        MLog.Print("DoStudentWork");
    //    }
    //}

    private void Start()
    {
        Person student = new Person();
        student.jobs.Add(new Student());
        student.jobs.Add(new Employee());
        student.DoJob();
    }

    public class Person
    {
        public List<Job> jobs = new List<Job>();

        public void DoJob()
        {
            foreach (var job in jobs)
            {
                job.DoWork();
            }
        }
    }

    public abstract class Job
    {
        public abstract void DoWork();
    }

    public class Employee : Job
    {
        public override void DoWork()
        {
            MLog.Print("DoEmployeeWork");
        }
    }

    public class Manager : Job
    {
        public override void DoWork()
        {
            MLog.Print("DoManagerWork");
        }
    }

    public class Student : Job
    {
        public override void DoWork()
        {
            MLog.Print("DoStudentWork");
        }
    }
}
