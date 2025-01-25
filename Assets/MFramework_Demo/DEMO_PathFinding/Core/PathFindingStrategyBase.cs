using MFramework;
using UnityEngine.Tilemaps;

public abstract class PathFindingStrategyBase : IPathFindingStrategy
{
    protected Tilemap m_tilemap;
    protected Grid m_startGrid;//���
    protected Grid m_endGrid;//�յ�

    protected float m_waitTime = 0.25f;//Э��ʱ�����

    public bool IsFinish { get; protected set; }

    public PathFindingStrategyBase()
    {
        m_waitTime = 0.25f / PathFindingInfo.Instance.Speed;
    }

    public void Reset()
    {
        MCoroutineManager.Instance.EndAllCoroutines();
        OnReset();//ʹ��ģ�巽��

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
    /// ���ñ��������������½�����һ��Ѱ·
    /// </summary>
    public abstract void OnReset();

    /// <summary>
    /// Ѱ·(A*/BFS/DFS/...)
    /// </summary>
    public abstract void OnPathFind();

}
