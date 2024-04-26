using MFramework;
using UnityEngine;

public class Test_StatePattern : MonoBehaviour
{
    private void Start()
    {
        Context context = new Context();
        
        StartState startState = new StartState();
        context.SetState(startState);
        startState.DoAction(context);

        MLog.Print($"Current State: {context.GetState().ToString()}");

        StopState stopState = new StopState();
        context.SetState(stopState);
        stopState.DoAction(context);

        MLog.Print($"Current State: {context.GetState().ToString()}");
    }

    public class Context
    {
        private State state;

        public Context()
        {
            state = null;
        }

        public void SetState(State state)
        {
            this.state = state;
        }
        public State GetState() => state;
    }

    public interface State
    {
        void DoAction(Context context);
    }
    public class StartState : State
    {
        public void DoAction(Context context)
        {
            MLog.Print("Change to StartState");
        }

        public override string ToString()
        {
            return "StartState";
        }
    }
    public class StopState : State
    {
        public void DoAction(Context context)
        {
            MLog.Print("Change to StopState");
        }

        public override string ToString()
        {
            return "StopState";
        }
    }
}
