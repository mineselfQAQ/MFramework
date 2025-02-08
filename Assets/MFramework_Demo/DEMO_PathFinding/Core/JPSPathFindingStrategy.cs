using MFramework;
using MFramework.DLC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPSPathFindingStrategy : PathFindingStrategyBase
{
    private List<Grid> finalPath = new List<Grid>();
    private HashSet<Grid> visited = new HashSet<Grid>();

    public JPSPathFindingStrategy() : base() { }

    public override string ToString()
    {
        return "JPS";
    }

    public override void OnReset()
    {
        //使用Clear替代重建，防止GC，而且路径一致情况下容量也是正好的
        finalPath.Clear();
        visited.Clear();

        IsFinish = false;
    }
    public override void OnPathFind(Action onFinish)
    {
        if (PathFindingInfo.Instance.dir == PathDir.Dir4)
        {
            MLog.Print($"{typeof(JPSPathFindingStrategy)}：JPS不支持四方向，请使用八方向", MLogType.Warning);
            return;
        }
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

            if (visited.Contains(curGrid)) yield break;

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

            Check(queue, curGrid);
        }
    }
    private void Check(MPriorityQueue<Grid, int> queue, Grid curGrid)
    {
        //只有八方向可用
        //使用GetGrid()不影响后续边缘判断，边上就是墙本就应该终止

        var rightGrid = curGrid.GetGrid(1, 0);
        var upGrid = curGrid.GetGrid(0, 1);
        var leftGrid = curGrid.GetGrid(-1, 0);
        var downGrid = curGrid.GetGrid(0, -1);

        Search(curGrid, rightGrid);
        Search(curGrid, upGrid);
        Search(curGrid, leftGrid);
        Search(curGrid, downGrid);

        var rightupGrid = curGrid.GetGrid(1, 1);
        var leftupGrid = curGrid.GetGrid(-1, 1);
        var leftdownGrid = curGrid.GetGrid(-1, -1);
        var rightdownGrid = curGrid.GetGrid(1, -1);

        Search(curGrid, rightupGrid);
        Search(curGrid, leftupGrid);
        Search(curGrid, leftdownGrid);
        Search(curGrid, rightdownGrid);
    }
    private void Search(Grid curGrid, Grid nextGrid)
    {
        if (JPSTool.IsInvalid(nextGrid)) return;

        int horizontalDir =  JPSTool.Dir(nextGrid.Pos.x, curGrid.Pos.x);
        int verticalDir = JPSTool.Dir(nextGrid.Pos.y, curGrid.Pos.y);

        Grid preGrid = curGrid;
        while (true)
        {

        }
    }
}
