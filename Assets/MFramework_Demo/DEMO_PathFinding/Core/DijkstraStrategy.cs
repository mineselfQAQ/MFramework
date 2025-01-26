using MFramework;
using MFramework.DLC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraStrategy : PathFindingStrategyBase
{
    private List<Grid> finalPath = new List<Grid>();

    public DijkstraStrategy() : base() { }

    public override string ToString()
    {
        return "Dijkstra";
    }

    public override void OnReset()
    {
        //ʹ��Clear����ؽ�����ֹGC������·��һ�����������Ҳ�����õ�
        finalPath.Clear();
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

            //if (visited.Contains(curGrid)) continue;//���жϣ������ٴβ���
            //if (curGrid == null) continue;//���жϣ������ٴβ���

            if (curGrid.Pos == m_endGrid.Pos)
            {
                //����Ѱ��·��
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

            PathFindingUtility.SetVisited(m_tilemap, curGrid);

            //�����¼���ڵ�
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
        if (nextGrid != null)
        {
            //�������ľ����Ƿ����(Խ�̵�·��Խ�Ƚ���)
            int newCost = curGrid.totalCost + nextGrid.cost;
            if (nextGrid.totalCost == 0 || newCost < nextGrid.totalCost)
            {
                nextGrid.ParentGrid = curGrid;
                nextGrid.totalCost = newCost;
                queue.Enqueue(nextGrid, newCost);
            }
        }
    }
}
