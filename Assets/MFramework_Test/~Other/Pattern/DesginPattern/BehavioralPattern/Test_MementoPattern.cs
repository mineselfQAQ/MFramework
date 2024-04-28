using MFramework;
using System.Collections.Generic;
using UnityEngine;

public class Test_MementoPattern : MonoBehaviour
{
    void Start()
    {
        Originator originator = new Originator();
        CareTaker careTaker = new CareTaker();

        originator.SetState("State 1");
        originator.SetState("State 2");
        careTaker.Add(originator.SaveStateToMemento());
        originator.SetState("State 3");
        careTaker.Add(originator.SaveStateToMemento());
        originator.SetState("State 4");

        MLog.Print(originator.GetState());
        MLog.Print(careTaker.Get(0));
        MLog.Print(careTaker.Get(1));
    }

    public class CareTaker
    {
        private List<Memento> mementoList = new List<Memento>();

        public void Add(Memento state)
        {
            mementoList.Add(state);
        }

        public Memento Get(int index)
        {
            return mementoList[index];
        }
    }

    public class Originator
    {
        private string state;

        public void SetState(string state)
        {
            this.state = state;
        }
        public string GetState() => state;

        public Memento SaveStateToMemento()
        {
            return new Memento(state);
        }
        public void GetStateFromMemento(Memento memento)
        {
            state = memento.GetState();
        }
    }

    public class Memento
    {
        private string state;

        public Memento(string state)
        {
            this.state = state;
        }

        public string GetState()
        {
            return state;
        }
    }
}
