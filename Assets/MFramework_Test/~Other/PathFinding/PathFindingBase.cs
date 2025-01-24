using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class PathFindingBase : IPathFinding
{
    protected Tilemap tilemap;
    protected readonly Grid startGrid;//起点
    protected readonly Grid endGrid;//终点

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
    /// 寻路(A*/BFS/DFS/...)
    /// </summary>
    public abstract void PathFind();

    /// <summary>
    /// 重置变量，可立即重新进行下一轮寻路
    /// </summary>
    public abstract void Reset();
}
