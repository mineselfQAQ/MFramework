using MFramework;
using UnityEngine.Tilemaps;

//TODO������ģʽ��
public abstract class PathFindingStrategyBase : IPathFindingStrategy
{
    protected Tilemap m_tilemap;
    protected readonly Grid m_startGrid;//���
    protected readonly Grid m_endGrid;//�յ�

    protected float m_waitTime = 0.25f;//Э��ʱ�����

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
        OnReset();//ʹ��ģ�巽��
    }

    public void FindPath()
    {
        OnPathFind();
    }

    /// <summary>
    /// Ѱ·(A*/BFS/DFS/...)
    /// </summary>
    public abstract void OnPathFind();

    /// <summary>
    /// ���ñ��������������½�����һ��Ѱ·
    /// </summary>
    public abstract void OnReset();
}
