using MFramework;
using UnityEngine.Tilemaps;

public abstract class PathFindingStrategyBase : IPathFindingStrategy
{
    protected Tilemap m_tilemap;
    protected Grid m_startGrid;//起点
    protected Grid m_endGrid;//终点

    protected float m_waitTime = 0.25f;//协程时间控制

    public bool IsFinish { get; protected set; }

    public PathFindingStrategyBase()
    {
        m_waitTime = 0.25f / PathFindingInfo.Instance.Speed;
    }

    public void Reset()
    {
        MCoroutineManager.Instance.EndAllCoroutines();
        OnReset();//使用模板方法

        m_tilemap = null;
        m_startGrid = null;
        m_endGrid = null;
    }

    public void FindPath(Tilemap tilemap, Grid startGrid, Grid endGrid)
    {
        m_tilemap = tilemap;
        m_startGrid = startGrid;
        m_endGrid = endGrid;

        OnPathFind();
    }

    /// <summary>
    /// 重置变量，可立即重新进行下一轮寻路
    /// </summary>
    public abstract void OnReset();

    /// <summary>
    /// 寻路(A*/BFS/DFS/...)
    /// </summary>
    public abstract void OnPathFind();

}
