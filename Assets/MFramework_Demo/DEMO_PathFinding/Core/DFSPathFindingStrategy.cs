using MFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DFSPathFindingStrategy : PathFindingStrategyBase
{
    private List<Grid> path = new List<Grid>();
    private List<Grid> finalPath = new List<Grid>();
    private HashSet<Grid> visited = new HashSet<Grid>();

    public DFSPathFindingStrategy() : base() { }

    public override string ToString()
    {
        return "DFS";
    }

    public override void OnReset()
    {
        //使用Clear替代重建，防止GC，而且路径一致情况下容量也是正好的
        path.Clear();
        finalPath.Clear();
        visited.Clear();
        //path = new List<Grid>();
        //finalPath = new List<Grid>();
        //visited = new HashSet<Grid>();

        IsFinish = false;
    }

    public override void OnPathFind(Action onFinish)
    {
        MCoroutineManager.Instance.StartCoroutine(DFS(onFinish), "PathFinding");
    }

    private IEnumerator DFS(Action onFinish)
    {
        yield return new WaitForSeconds(1);
        
        yield return MCoroutineManager.Instance.StartCoroutine(DFSTraverse(m_startGrid, null), "PathFindingInternal");

        for (int i = 0; i < finalPath.Count; i++)
        {
            yield return new WaitForSeconds(m_waitTime);
            PathFindingUtility.SetAnyFinal(m_tilemap, finalPath[i]);
        }
        onFinish?.Invoke();
    }
    private IEnumerator DFSTraverse(Grid curGrid, Grid parentGrid)
    {
        if (IsFinish) yield break;//已完成就回退
        yield return new WaitForSeconds(m_waitTime);

        if (curGrid == null) yield break;//未获取到grid，即撞墙或出界了
        //Parent不需要额外判断，因为visited已经记录了
        //if (parentGrid != null && parentGrid.ParentGrid == grid) yield break;//回头了，不应该进行
        if (visited.Contains(curGrid)) yield break;

        curGrid.ParentGrid = parentGrid;
        path.Add(curGrid);
        visited.Add(curGrid);
        PathFindingUtility.SetVisited(m_tilemap, curGrid);

        //完成条件
        if (curGrid.Pos == m_endGrid.Pos)
        {
            IsFinish = true;
            finalPath = new List<Grid>(path);//复制存储
            yield break;
        }

        if (PathFindingInfo.Instance.dir == PathDir.Dir4)
        {
            yield return DFSTraverse(curGrid.GetGrid(1, 0), curGrid);
            yield return DFSTraverse(curGrid.GetGrid(0, 1), curGrid);
            yield return DFSTraverse(curGrid.GetGrid(-1, 0), curGrid);
            yield return DFSTraverse(curGrid.GetGrid(0, -1), curGrid);
        }
        else if (PathFindingInfo.Instance.dir == PathDir.Dir8)
        {
            yield return DFSTraverse(curGrid.GetGrid(1, 0), curGrid);
            yield return DFSTraverse(curGrid.GetGrid(1, 1), curGrid);
            yield return DFSTraverse(curGrid.GetGrid(0, 1), curGrid);
            yield return DFSTraverse(curGrid.GetGrid(-1, 1), curGrid);
            yield return DFSTraverse(curGrid.GetGrid(-1, 0), curGrid);
            yield return DFSTraverse(curGrid.GetGrid(-1, -1), curGrid);
            yield return DFSTraverse(curGrid.GetGrid(0, -1), curGrid);
            yield return DFSTraverse(curGrid.GetGrid(1, -1), curGrid);
        }

        path.Remove(curGrid);
    }
}
