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
        //ʹ��Clear����ؽ�����ֹGC������·��һ�����������Ҳ�����õ�
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
        if (IsFinish) yield break;//����ɾͻ���
        yield return new WaitForSeconds(m_waitTime);

        if (curGrid == null) yield break;//δ��ȡ��grid����ײǽ�������
        //Parent����Ҫ�����жϣ���Ϊvisited�Ѿ���¼��
        //if (parentGrid != null && parentGrid.ParentGrid == grid) yield break;//��ͷ�ˣ���Ӧ�ý���
        if (visited.Contains(curGrid)) yield break;

        curGrid.ParentGrid = parentGrid;
        path.Add(curGrid);
        visited.Add(curGrid);
        PathFindingUtility.SetVisited(m_tilemap, curGrid);

        //�������
        if (curGrid.Pos == m_endGrid.Pos)
        {
            IsFinish = true;
            finalPath = new List<Grid>(path);//���ƴ洢
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
