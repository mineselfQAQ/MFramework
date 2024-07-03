using UnityEngine;

public class Test_StrategyPattern : MonoBehaviour
{
    private void Start()
    {
        Context context = new Context(new OperationAdd());
        context.ExecuteStrategy(1, 2);

        context.SetStrategy(new OperationSub());
        context.ExecuteStrategy(1, 2);
    }

    public class Context
    {
        private Strategy strategy;

        public Context(Strategy strategy)
        {
            this.strategy = strategy;
        }

        public void SetStrategy(Strategy strategy)
        {
            this.strategy = strategy;
        }

        public int ExecuteStrategy(int a, int b)
        {
            return strategy.DoOperation(a, b);
        }
    }

    public interface Strategy
    {
        int DoOperation(int a, int b);
    }
    public class OperationAdd : Strategy
    {
        public int DoOperation(int a, int b)
        {
            return a + b;
        }
    }
    public class OperationSub : Strategy
    {
        public int DoOperation(int a, int b)
        {
            return a - b;
        }
    }
}
