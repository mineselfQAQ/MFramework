using System;
using UnityEngine;

public class Test_StrategyPattern : MonoBehaviour
{
    private void Start()
    {
        PathfindingContext context = new PathfindingContext();

        Vector2 start = new Vector2(0, 0);
        Vector2 goal = new Vector2(10, 10);

        // ʹ�� A* �㷨
        context.SetStrategy(new AStarPathfinding());
        context.ExecutePathfinding(start, goal);

        // �л�Ϊ BFS �㷨
        context.SetStrategy(new BFSPathfinding());
        context.ExecutePathfinding(start, goal);

        // �л�Ϊ��ֱ��Ѱ·
        context.SetStrategy(new StraightLinePathfinding());
        context.ExecutePathfinding(start, goal);
    }

    // ���Խӿڣ�����Ѱ·�㷨
    public interface IPathfindingStrategy
    {
        void FindPath(Vector2 start, Vector2 goal);
    }

    // ������ԣ�A* �㷨
    public class AStarPathfinding : IPathfindingStrategy
    {
        public void FindPath(Vector2 start, Vector2 goal)
        {
            Console.WriteLine($"ʹ�� A* �㷨�� {start} �� {goal} Ѱ·��");
            // A* �㷨��ʵ���߼�...
        }
    }

    // ������ԣ��������������BFS��
    public class BFSPathfinding : IPathfindingStrategy
    {
        public void FindPath(Vector2 start, Vector2 goal)
        {
            Console.WriteLine($"ʹ�� BFS �㷨�� {start} �� {goal} Ѱ·��");
            // BFS �㷨��ʵ���߼�...
        }
    }

    // ������ԣ���ֱ��Ѱ·
    public class StraightLinePathfinding : IPathfindingStrategy
    {
        public void FindPath(Vector2 start, Vector2 goal)
        {
            Console.WriteLine($"ʹ�ü�ֱ�ߴ� {start} �� {goal} Ѱ·��");
            // ��ֱ��Ѱ·�߼�...
        }
    }

    // �������ࣺʹ�ò��Զ���Ļ���
    public class PathfindingContext
    {
        private IPathfindingStrategy _strategy;

        // ���õ�ǰ����
        public void SetStrategy(IPathfindingStrategy strategy)
        {
            _strategy = strategy;
        }

        // ִ��Ѱ·
        public void ExecutePathfinding(Vector2 start, Vector2 goal)
        {
            if (_strategy == null)
            {
                Console.WriteLine("δ����Ѱ·���ԣ�");
                return;
            }

            _strategy.FindPath(start, goal);
        }
    }
}
