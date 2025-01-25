using MFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DFSPathFindingStrategy : PathFindingStrategyBase
{
    private List<Grid> path = new List<Grid>();
    private List<Grid> finalPath = new List<Grid>();
    private HashSet<Grid> visited = new HashSet<Grid>();

    public DFSPathFindingStrategy(Tilemap tilemap, Grid startGrid, Grid endGrid)
        : base(tilemap, startGrid, endGrid) { }

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

        m_isFinish = false;
    }

    public override void OnPathFind()
    {
        MCoroutineManager.Instance.StartCoroutine(DFS(), "PathFinding");
    }

    private IEnumerator DFS()
    {
        yield return new WaitForSeconds(1);
        
        yield return MCoroutineManager.Instance.StartCoroutine(DFSTraverse(m_startGrid, null), "PathFindingInternal");

        for (int i = 1; i < finalPath.Count; i++)
        {
            yield return new WaitForSeconds(m_waitTime);
            m_tilemap.SetTile(finalPath[i].posInternal, PathFindingInfo.Instance.FinalTile);
        }
    }
    private IEnumerator DFSTraverse(Grid grid, Grid parentGrid)
    {
        if (m_isFinish) yield break;//已完成就回退
        yield return new WaitForSeconds(m_waitTime);

        if (grid == null) yield break;//未获取到grid，即撞墙或出界了
        //Parent不需要额外判断，因为visited已经记录了
        //if (parentGrid != null && parentGrid.ParentGrid == grid) yield break;//回头了，不应该进行
        if (visited.Contains(grid)) yield break;

        //完成条件
        if (grid.Pos == m_endGrid.Pos)
        {
            m_isFinish = true;
            finalPath = new List<Grid>(path);//复制存储
            yield break;
        }

        grid.ParentGrid = parentGrid;
        path.Add(grid);
        visited.Add(grid);
        if (grid.type == GridType.Path)
        {
            m_tilemap.SetTile(grid.posInternal, PathFindingInfo.Instance.VisitedTile);
        }

        yield return DFSTraverse(grid.GetGrid(1, 0), grid);
        yield return DFSTraverse(grid.GetGrid(0, 1), grid);
        yield return DFSTraverse(grid.GetGrid(-1, 0), grid);
        yield return DFSTraverse(grid.GetGrid(0, -1), grid);

        path.Remove(grid);
    }
}
