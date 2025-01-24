using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class PathFindingBase : IPathFinding
{
    protected Tilemap tilemap;
    protected readonly Grid startGrid;//���
    protected readonly Grid endGrid;//�յ�

    protected bool isFinish = false;
    protected float waitTime = 0.25f;

    public PathFindingBase(Tilemap tilemap, Grid startGrid, Grid endGrid)
    {
        this.tilemap = tilemap;
        this.startGrid = startGrid;
        this.endGrid = endGrid;

        waitTime = 0.25f / PathFindingInfo.Instance.Speed;
    }
    
    /// <summary>
    /// Ѱ·(A*/BFS/DFS/...)
    /// </summary>
    public abstract void PathFind();

    /// <summary>
    /// ���ñ��������������½�����һ��Ѱ·
    /// </summary>
    public abstract void Reset();
}
