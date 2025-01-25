using MFramework;
using MFramework.DLC;
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

    public override void OnPathFind()
    {
        MCoroutineManager.Instance.StartCoroutine(BFS(), "PathFinding");
    }
    private IEnumerator BFS()
    {
        yield return new WaitForSeconds(1);

        yield return MCoroutineManager.Instance.StartCoroutine(BFSTraverse(m_startGrid), "PathFindingInternal");

        for (int i = 0; i < finalPath.Count; i++)
        {
            yield return new WaitForSeconds(m_waitTime);
            m_tilemap.SetTile(finalPath[i].posInternal, PathFindingInfo.Instance.FinalTile);
        }
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

            if (curGrid.Pos == m_endGrid.Pos)
            {
                //反向寻找路径
                Grid tempGrid = curGrid.ParentGrid;
                while (tempGrid.Pos != m_startGrid.Pos)
                {
                    finalPath.Add(tempGrid);
                    tempGrid = tempGrid.ParentGrid;
                }
                finalPath.Reverse();

                IsFinish = true;

                break;
            }

            if (curGrid.type == GridType.Path)
            {
                m_tilemap.SetTile(curGrid.posInternal, PathFindingInfo.Instance.VisitedTile);
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
    private void Enqueue(MPriorityQueue<Grid, int> queue, Grid curGrid, Grid nextGrid)
    {
        if (nextGrid != null && !visited.Contains(nextGrid))
        {
            visited.Add(nextGrid);//提前加入

            int priority = Heuristic(nextGrid.Pos);

            nextGrid.ParentGrid = curGrid;
            queue.Enqueue(nextGrid, priority);
        }
    }
    private int Heuristic(Vector2Int posA, Vector2Int posB)
    {
        return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
    }
    private int Heuristic(Vector2Int posA)
    {
        Vector2Int posB = m_endGrid.Pos;
        return Heuristic(posA, posB);
    }
}
