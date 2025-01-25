using UnityEngine.Tilemaps;

public class PathFindingSystem
{
    protected bool m_Dirty;

    public IPathFindingStrategy Strategy { get; private set; }

    public PathFindingSystem()
    {
        m_Dirty = false;
    }
    public PathFindingSystem(IPathFindingStrategy defaultStrategy)
    {
        m_Dirty = false;
        Strategy = defaultStrategy;
    }

    public void SetStrategy(IPathFindingStrategy strategy)
    {
        Strategy = strategy;
    }

    public void ExecutePathfinding(Tilemap tilemap, Grid startGrid, Grid endGrid)
    {
        if (Strategy == null) return;

        if (m_Dirty) Strategy.Reset();

        m_Dirty = true;
        Strategy.FindPath(tilemap, startGrid, endGrid);
    }
}
