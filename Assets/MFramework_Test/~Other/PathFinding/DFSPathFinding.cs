using MFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DFSPathFinding : IPathFinding
{
    private Tilemap tilemap;
    private readonly Grid startGrid;//���
    private readonly Grid endGrid;//�յ�

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
        if (isFinish) yield break;//����ɾͻ���
        yield return new WaitForSeconds(waitTime);

        if (grid == null) yield break;//δ��ȡ��grid����ײǽ�������
        //Parent����Ҫ�����жϣ���Ϊvisited�Ѿ���¼��
        //if (parentGrid != null && parentGrid.ParentGrid == grid) yield break;//��ͷ�ˣ���Ӧ�ý���
        if (visited.Contains(grid)) yield break;

        //�������
        if (grid.Pos == endGrid.Pos)
        {
            isFinish = true;
            finalPath = new List<Grid>(path);//���ƴ洢
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
