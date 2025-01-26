using MFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFSPathFindingStrategy : PathFindingStrategyBase
{
    private List<Grid> finalPath = new List<Grid>();
    private HashSet<Grid> visited = new HashSet<Grid>();

    public BFSPathFindingStrategy() : base() { }

    public override string ToString()
    {
        return "BFS";
    }

    public override void OnReset()
    {
        //ʹ��Clear����ؽ�����ֹGC������·��һ�����������Ҳ�����õ�
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
        Queue<Grid> queue = new Queue<Grid>();
        queue.Enqueue(grid);

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
    private void Enqueue(Queue<Grid> queue, Grid curGrid, Grid nextGrid)
    {
        if (nextGrid != null && !visited.Contains(nextGrid))
        {
            visited.Add(nextGrid);//��ǰ����

            nextGrid.ParentGrid = curGrid;
            queue.Enqueue(nextGrid);
        }
    }
}
