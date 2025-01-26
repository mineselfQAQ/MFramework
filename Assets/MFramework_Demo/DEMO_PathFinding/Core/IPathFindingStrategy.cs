using System;
using UnityEngine.Tilemaps;

public interface IPathFindingStrategy
{
    void FindPath(Tilemap tilemap, Grid startGrid, Grid endGrid, Action onFinish);
    void Reset();
}
