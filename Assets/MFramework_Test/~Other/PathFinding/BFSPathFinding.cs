using MFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BFSPathFinding : IPathFinding
{
    private Tilemap tilemap;
    private readonly Grid startGrid;//起点
    private readonly Grid endGrid;//终点

    private List<Grid> finalPath = new List<Grid>();
    private HashSet<Grid> visited = new HashSet<Grid>();

    private float waitTime = 0.25f;

    public BFSPathFinding(Tilemap tilemap, Grid startGrid, Grid endGrid)
    {
        this.tilemap = tilemap;
        this.startGrid = startGrid;
        this.endGrid = endGrid;

        waitTime = 0.25f / PathFindingInfo.Instance.Speed;
    }

    public void Reset()
    {
        finalPath = new List<Grid>();
        visited = new HashSet<Grid>();
    }

    public void PathFind()
    {
        MCoroutineManager.Instance.StartCoroutine(BFS(), "PathFinding");
    }

    private IEnumerator BFS()
    {
        yield return new WaitForSeconds(1);

        yield return MCoroutineManager.Instance.StartCoroutine(BFSTraverse(startGrid), "PathFindingInternal");

        for (int i = 0; i < finalPath.Count; i++)
        {
            yield return new WaitForSeconds(waitTime);
            tilemap.SetTile(finalPath[i].posInternal, PathFindingInfo.Instance.FinalTile);
        }
    }
    private IEnumerator BFSTraverse(Grid grid)
    {
        Queue<Grid> queue = new Queue<Grid>();
        queue.Enqueue(grid);

        while (queue.Count > 0)
        {
            yield return new WaitForSeconds(waitTime);

            Grid curGrid = queue.Dequeue();

            //if (visited.Contains(curGrid)) continue;//已判断，无需再次操作
            //if (curGrid == null) continue;//已判断，无需再次操作

            if (curGrid.Pos == endGrid.Pos)
            {
                //反向寻找路径
                Grid tempGrid = curGrid.ParentGrid;
                while (tempGrid.Pos != startGrid.Pos)
                {
                    finalPath.Add(tempGrid);
                    tempGrid = tempGrid.ParentGrid;
                }
                finalPath.Reverse();

                break;
            }

            visited.Add(curGrid);
            if (curGrid.type == GridType.Path)
            {
                tilemap.SetTile(curGrid.posInternal, PathFindingInfo.Instance.VisitedTile);
            }

            //反向记录父节点
            var rightGrid = curGrid.GetGrid(1, 0);
            var upGrid = curGrid.GetGrid(0, 1);
            var leftGrid = curGrid.GetGrid(-1, 0);
            var downGrid = curGrid.GetGrid(0, -1);
            Enqueue(queue, curGrid, rightGrid);
            Enqueue(queue, curGrid, upGrid);
            Enqueue(queue, curGrid, leftGrid);
            Enqueue(queue, curGrid, downGrid);
        }
    }
    private void Enqueue(Queue<Grid> queue, Grid curGrid, Grid nextGrid)
    {
        if (nextGrid != null && !visited.Contains(nextGrid))
        {
            nextGrid.ParentGrid = curGrid;
            queue.Enqueue(nextGrid);
        }
    }
}
