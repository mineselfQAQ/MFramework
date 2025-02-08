using MFramework;
using MFramework.DLC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedyBFSPathFindingStrategy : PathFindingStrategyBase
{
    private List<Grid> finalPath = new List<Grid>();
    private HashSet<Grid> visited = new HashSet<Grid>();

    public GreedyBFSPathFindingStrategy() : base() { }

    public override string ToString()
    {
        return "GreedyBFS";
    }

    public override void OnReset()
    {
        //使用Clear替代重建，防止GC，而且路径一致情况下容量也是正好的
        finalPath.Clear();
        visited.Clear();
        //finalPath = new List<Grid>();
        //visited = new HashSet<Grid>();

        IsFinish = false;
    }

    public override void OnPathFind(Action onFinish)
    {
        MCoroutineManager.Instance.StartCoroutine(BFS(onFinish), "PathFinding");
    }
    private IEnumerator BFS(Action onFinish)
    {
        yield return new WaitForSeconds(1);

        yield return MCoroutineManager.Instance.StartCoroutine(BFSTraverse(m_startGrid), "PathFindingInternal");

        for (int i = 0; i < finalPath.Count; i++)
        {
            yield return new WaitForSeconds(m_waitTime);
            PathFindingUtility.SetAnyFinal(m_tilemap, finalPath[i]);
        }
        onFinish?.Invoke();
    }
    private IEnumerator BFSTraverse(Grid grid)
    {
        MPriorityQueue<Grid, int> queue = new MPriorityQueue<Grid, int>();
        queue.Enqueue(grid, 0);

        while (queue.Count > 0)
        {
            yield return new WaitForSeconds(m_waitTime);

            Grid curGrid = queue.Dequeue();

            //if (visited.Contains(curGrid)) continue;//已判断，无需再次操作
            //if (curGrid == null) continue;//已判断，无需再次操作

            PathFindingUtility.SetVisited(m_tilemap, curGrid);

            if (curGrid.Pos == m_endGrid.Pos)
            {
                //反向寻找路径
                Grid tempGrid = curGrid;
                while (tempGrid.Pos != m_startGrid.Pos)
                {
                    finalPath.Add(tempGrid);
                    tempGrid = tempGrid.ParentGrid;
                }
                finalPath.Add(tempGrid);
                finalPath.Reverse();

                IsFinish = true;

                break;
            }

            Enqueue(queue, curGrid);
        }
    }
    private void Enqueue(MPriorityQueue<Grid, int> queue, Grid curGrid)
    {
        if (PathFindingInfo.Instance.dir == PathDir.Dir4)
        {
            var rightGrid = curGrid.GetGrid(1, 0);
            var upGrid = curGrid.GetGrid(0, 1);
            var leftGrid = curGrid.GetGrid(-1, 0);
            var downGrid = curGrid.GetGrid(0, -1);
            Enqueue(queue, curGrid, rightGrid);
            Enqueue(queue, curGrid, upGrid);
            Enqueue(queue, curGrid, leftGrid);
            Enqueue(queue, curGrid, downGrid);
        }
        else if (PathFindingInfo.Instance.dir == PathDir.Dir8)
        {
            var rightGrid = curGrid.GetGrid(1, 0);
            var rightupGrid = curGrid.GetGrid(1, 1);
            var upGrid = curGrid.GetGrid(0, 1);
            var leftupGrid = curGrid.GetGrid(-1, 1);
            var leftGrid = curGrid.GetGrid(-1, 0);
            var leftdownGrid = curGrid.GetGrid(-1, -1);
            var downGrid = curGrid.GetGrid(0, -1);
            var rightdownGrid = curGrid.GetGrid(1, -1);
            Enqueue(queue, curGrid, rightGrid);
            Enqueue(queue, curGrid, rightupGrid);
            Enqueue(queue, curGrid, upGrid);
            Enqueue(queue, curGrid, leftupGrid);
            Enqueue(queue, curGrid, leftGrid);
            Enqueue(queue, curGrid, leftdownGrid);
            Enqueue(queue, curGrid, downGrid);
            Enqueue(queue, curGrid, rightdownGrid);
        }
    }
    private void Enqueue(MPriorityQueue<Grid, int> queue, Grid curGrid, Grid nextGrid)
    {
        if (nextGrid != null && !visited.Contains(nextGrid))
        {
            visited.Add(nextGrid);//提前加入

            int priority = PathFindingUtility.Heuristic(nextGrid.Pos, m_endGrid.Pos);

            nextGrid.ParentGrid = curGrid;//反向记录父节点
            queue.Enqueue(nextGrid, priority);
        }
    }
}
