using System;
using UnityEngine;

public class Test_StrategyPattern : MonoBehaviour
{
    private void Start()
    {
        PathfindingContext context = new PathfindingContext();

        Vector2 start = new Vector2(0, 0);
        Vector2 goal = new Vector2(10, 10);

        // 使用 A* 算法
        context.SetStrategy(new AStarPathfinding());
        context.ExecutePathfinding(start, goal);

        // 切换为 BFS 算法
        context.SetStrategy(new BFSPathfinding());
        context.ExecutePathfinding(start, goal);

        // 切换为简单直线寻路
        context.SetStrategy(new StraightLinePathfinding());
        context.ExecutePathfinding(start, goal);
    }

    // 策略接口：定义寻路算法
    public interface IPathfindingStrategy
    {
        void FindPath(Vector2 start, Vector2 goal);
    }

    // 具体策略：A* 算法
    public class AStarPathfinding : IPathfindingStrategy
    {
        public void FindPath(Vector2 start, Vector2 goal)
        {
            Console.WriteLine($"使用 A* 算法从 {start} 到 {goal} 寻路。");
            // A* 算法的实现逻辑...
        }
    }

    // 具体策略：广度优先搜索（BFS）
    public class BFSPathfinding : IPathfindingStrategy
    {
        public void FindPath(Vector2 start, Vector2 goal)
        {
            Console.WriteLine($"使用 BFS 算法从 {start} 到 {goal} 寻路。");
            // BFS 算法的实现逻辑...
        }
    }

    // 具体策略：简单直线寻路
    public class StraightLinePathfinding : IPathfindingStrategy
    {
        public void FindPath(Vector2 start, Vector2 goal)
        {
            Console.WriteLine($"使用简单直线从 {start} 到 {goal} 寻路。");
            // 简单直线寻路逻辑...
        }
    }

    // 上下文类：使用策略对象的环境
    public class PathfindingContext
    {
        private IPathfindingStrategy _strategy;

        // 设置当前策略
        public void SetStrategy(IPathfindingStrategy strategy)
        {
            _strategy = strategy;
        }

        // 执行寻路
        public void ExecutePathfinding(Vector2 start, Vector2 goal)
        {
            if (_strategy == null)
            {
                Console.WriteLine("未设置寻路策略！");
                return;
            }

            _strategy.FindPath(start, goal);
        }
    }
}
