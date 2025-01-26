using System;
using UnityEngine.Tilemaps;

public class PathFindingSystem
{
    private bool m_IsDirty;
    private bool m_IsFinish;

    public bool IsDirty => m_IsDirty;
    public bool IsFinish => m_IsFinish;

    public IPathFindingStrategy Strategy { get; private set; }

    public PathFindingSystem()
    {
        m_IsDirty = false;
        m_IsFinish = true;//未开始算一种完成
    }
    public PathFindingSystem(IPathFindingStrategy defaultStrategy)
    {
        m_IsDirty = false;
        m_IsFinish = true;//未开始算一种完成
        Strategy = defaultStrategy;
    }

    public void SetStrategy(IPathFindingStrategy strategy)
    {
        Strategy = strategy;
    }

    public void Execute(Tilemap tilemap, Grid startGrid, Grid endGrid, Action onFinish = null)
    {
        if (Strategy == null) return;

        if (m_IsDirty) Strategy.Reset();

        m_IsDirty = true;
        m_IsFinish = false;
        Strategy.FindPath(tilemap, startGrid, endGrid, () => 
        {
            m_IsFinish = true;
        });
    }
}
