using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class PathFindingUtility
{
    private static Dictionary<GridType, TileBase> SetVisitedMap;

    public static void SetVisited(Tilemap tilemap, Grid grid)
    {
        if (SetVisitedMap == null)
        {
            SetVisitedMap = new Dictionary<GridType, TileBase>()
            {
                { GridType.Path, PathFindingInfo.Instance.VisitedTile },
                { GridType.Barrier, PathFindingInfo.Instance.VisitedBarrier }
            };
        }

        if (!SetVisitedMap.ContainsKey(grid.type)) return;
        tilemap.SetTile(grid.posInternal, SetVisitedMap[grid.type]);
    }

    public static void SetAnyFinal(Tilemap tilemap, Grid grid)
    {
        tilemap.SetTile(grid.posInternal, PathFindingInfo.Instance.FinalTile);
    }
}
