using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class PathFindingUtility
{
    #region Tilemap贴图设置
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

    public static void SetStart(Tilemap tilemap, Vector3Int pos)
    {
        tilemap.SetTile(pos, PathFindingInfo.Instance.StartMark);
    }
    public static void SetEnd(Tilemap tilemap, Vector3Int pos)
    {
        tilemap.SetTile(pos, PathFindingInfo.Instance.EndMark);
    }
    public static void SetNull(Tilemap tilemap, Vector3Int pos)
    {
        tilemap.SetTile(pos, null);
    }
    public static MarkType SetStartEndMark(Tilemap tilemap, Vector3Int pos, Map curMap)
    {
        Vector3Int mapPos = pos - curMap.tilemap.origin;
        if(mapPos.x < 0 || mapPos.y < 0 || mapPos.x >= curMap.tilemap.size.x || mapPos.y >= curMap.tilemap.size.y)
            return MarkType.Error;//出界

        Tile tile = tilemap.GetTile<Tile>(pos);

        //Start->End->Null->Start->...
        if (tile == null)
        {
            SetStart(tilemap, pos);
            return MarkType.Start;
        }
        string name = tile.name;
        if (name == PathFindingInfo.startGridName)
        {
            SetEnd(tilemap, pos);
            return MarkType.End;
        }
        else if (name == PathFindingInfo.endGridName)
        {
            SetNull(tilemap, pos);
            return MarkType.Null;
        }

        return MarkType.Error;
    }
    #endregion

    #region 一般函数
    /// <summary>
    /// 启发式算法(两点间距离)
    /// </summary>
    public static int Heuristic(Vector2Int posA, Vector2Int posB)
    {
        return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
    }
    #endregion
}
