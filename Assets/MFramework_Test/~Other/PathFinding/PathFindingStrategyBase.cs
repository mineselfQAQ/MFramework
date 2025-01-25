using MFramework;
using UnityEngine.Tilemaps;

//TODO：策略模式？
public abstract class PathFindingStrategyBase : IPathFindingStrategy
{
    protected Tilemap m_tilemap;
    protected readonly Grid m_startGrid;//起点
    protected readonly Grid m_endGrid;//终点

    protected float m_waitTime = 0.25f;//协程时间控制

    protected bool m_isFinish = false;

    public PathFindingStrategyBase(Tilemap tilemap, Grid startGrid, Grid endGrid)
    {
        this.m_tilemap = tilemap;
        this.m_startGrid = startGrid;
        this.m_endGrid = endGrid;

        m_waitTime = 0.25f / PathFindingInfo.Instance.Speed;
    }

    public void Reset()
    {
        MCoroutineManager.Instance.EndAllCoroutines();
        OnReset();//使用模板方法
    }

    public void FindPath()
    {
        OnPathFind();
    }

    /// <summary>
    /// 寻路(A*/BFS/DFS/...)
    /// </summary>
    public abstract void OnPathFind();

    /// <summary>
    /// 重置变量，可立即重新进行下一轮寻路
    /// </summary>
    public abstract void OnReset();
}
