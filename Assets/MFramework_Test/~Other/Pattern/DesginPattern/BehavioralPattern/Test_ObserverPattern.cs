using MFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Test_ObserverPattern : MonoBehaviour
{
    private void Start()
    {
        Subject subject = new Subject();

        new BinaryObserver(subject);
        new OctalObserver(subject);
        new HexaObserver(subject);

        subject.SetState(15);
        subject.SetState(10);
    }

    public class Subject
    {
        private List<Observer> observers = new List<Observer>();
        private int state;

        public int GetState => state;
        public void SetState(int state)
        {
            this.state = state;
            NotifyAllObservers();
        }

        public void Attach(Observer observer)
        {
            observers.Add(observer);
        }

        public void NotifyAllObservers()
        {
            foreach (var observer in observers)
            {
                observer.Update();
            }
        }
    }

    public abstract class Observer
    {
        protected Subject subject;//±ªπ€≤Ï’ﬂ

        public abstract void Update();
    }
    public class BinaryObserver : Observer
    {
        public BinaryObserver(Subject subject)
        {
            this.subject = subject;
            this.subject.Attach(this);
        }

        public override void Update()
        {
            MLog.Print($"Binary: {(subject.GetState).ToBinaryString()}");
        }
    }
    public class OctalObserver : Observer
    {
        public OctalObserver(Subject subject)
        {
            this.subject = subject;
            this.subject.Attach(this);
        }

        public override void Update()
        {
            MLog.Print($"Octal: {subject.GetState}");
        }
    }
    public class HexaObserver : Observer
    {
        public HexaObserver(Subject subject)
        {
            this.subject = subject;
            this.subject.Attach(this);
        }

        public override void Update()
        {
            MLog.Print($"Hex: {Convert.ToString(subject.GetState, 16)}");
        }
    }
}
