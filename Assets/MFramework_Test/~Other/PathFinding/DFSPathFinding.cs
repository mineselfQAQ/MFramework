using MFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DFSPathFinding : IPathFinding
{
    private Tilemap tilemap;
    private readonly Grid startGrid;//起点
    private readonly Grid endGrid;//终点

    private List<Grid> path = new List<Grid>();
    private List<Grid> finalPath = new List<Grid>();
    private HashSet<Grid> visited = new HashSet<Grid>();

    private bool isFinish = false;
    private float waitTime = 0.25f;

    public DFSPathFinding(Tilemap tilemap, Grid startGrid, Grid endGrid)
    {
        this.tilemap = tilemap;
        this.startGrid = startGrid;
        this.endGrid = endGrid;

        waitTime = 0.25f / PathFindingInfo.Instance.Speed;
    }

    public void Reset()
    {
        path = new List<Grid>();
        finalPath = new List<Grid>();
        visited = new HashSet<Grid>();

        isFinish = false;
    }

    public void PathFind()
    {
        MCoroutineManager.Instance.StartCoroutine(DFS(), "PathFinding");
    }

    private IEnumerator DFS()
    {
        yield return new WaitForSeconds(1);
        
        yield return MCoroutineManager.Instance.StartCoroutine(DFSTraverse(startGrid, null), "PathFindingInternal");

        for (int i = 1; i < finalPath.Count; i++)
        {
            yield return new WaitForSeconds(waitTime);
            tilemap.SetTile(finalPath[i].posInternal, PathFindingInfo.Instance.FinalTile);
        }
    }
    private IEnumerator DFSTraverse(Grid grid, Grid parentGrid)
    {
        if (isFinish) yield break;//已完成就回退
        yield return new WaitForSeconds(waitTime);

        if (grid == null) yield break;//未获取到grid，即撞墙或出界了
        //Parent不需要额外判断，因为visited已经记录了
        //if (parentGrid != null && parentGrid.ParentGrid == grid) yield break;//回头了，不应该进行
        if (visited.Contains(grid)) yield break;

        //完成条件
        if (grid.Pos == endGrid.Pos)
        {
            isFinish = true;
            finalPath = new List<Grid>(path);//复制存储
            yield break;
        }

        grid.ParentGrid = parentGrid;
        path.Add(grid);
        visited.Add(grid);
        if (grid.type == GridType.Path)
        {
            tilemap.SetTile(grid.posInternal, PathFindingInfo.Instance.VisitedTile);
        }

        yield return DFSTraverse(grid.GetGrid(1, 0), grid);
        yield return DFSTraverse(grid.GetGrid(0, 1), grid);
        yield return DFSTraverse(grid.GetGrid(-1, 0), grid);
        yield return DFSTraverse(grid.GetGrid(0, -1), grid);

        path.Remove(grid);
    }
}
