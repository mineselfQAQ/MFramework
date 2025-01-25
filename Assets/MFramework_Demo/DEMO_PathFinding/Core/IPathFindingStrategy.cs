using UnityEngine.Tilemaps;

public interface IPathFindingStrategy
{
    void FindPath(Tilemap tilemap, Grid startGrid, Grid endGrid);
    void Reset();
}
