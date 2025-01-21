using MFramework;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test_PathFinding : MonoBehaviour
{
    private Tilemap tilemap;
    private Grid startGrid;
    private Grid endGrid;

    private Grid[,] originGridMap;
    private Grid[,] gridMap;

    private IPathFinding DFSPathFinding;
    private IPathFinding BFSPathFinding;

    private bool dirty = false;

    private void Awake()
    {
        //**注意**
        //如果进行过擦除Tile操作，这不会自动重计算Bound，需要：
        //1.点击Tilemap的三点中的Compress Tilemap Bounds
        //2.tilemap.CompressBounds();
        tilemap = PathFindingInfo.Instance.Tilemap;
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;

        originGridMap = new Grid[bounds.size.x, bounds.size.y];
        gridMap = new Grid[bounds.size.x, bounds.size.y];

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                int xPos = bounds.xMin + x;
                int yPos = bounds.yMin + y;

                Vector3Int position = new Vector3Int(xPos, yPos, 0);
                Tile tile = tilemap.GetTile<Tile>(position);
                GridType type = Enum.Parse<GridType>(tile.name);
                gridMap[x, y] = new Grid(tile, type, gridMap, position, x, y);

                if (type == GridType.Start) startGrid = gridMap[x, y];
                else if (type == GridType.End) endGrid = gridMap[x, y];
            }
        }
        originGridMap = gridMap;//保存原始状态，在结束后恢复

        DFSPathFinding = new DFSPathFinding(PathFindingInfo.Instance.Tilemap, startGrid, endGrid);
        BFSPathFinding = new BFSPathFinding(PathFindingInfo.Instance.Tilemap, startGrid, endGrid);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (dirty) { ResetColor(); dirty = false; }
            MCoroutineManager.Instance.EndAllCoroutines();
            DFSPathFinding.Reset();

            DFSPathFinding.PathFind();
            dirty = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (dirty) { ResetColor(); dirty = false; }
            MCoroutineManager.Instance.EndAllCoroutines();
            BFSPathFinding.Reset();

            BFSPathFinding.PathFind();
            dirty = true;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetColor();
            dirty = false;
        }
    }

    private void OnApplicationQuit()
    {
        ResetColor();
    }

    private void ResetColor()
    {
        for (int x = 0; x < originGridMap.GetLength(0); x++)
        {
            for (int y = 0; y < originGridMap.GetLength(1); y++)
            {
                var grid = originGridMap[x, y];
                tilemap.SetTile(grid.posInternal, grid.tile);
            }
        }
    }
}
