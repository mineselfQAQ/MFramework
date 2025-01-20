using MFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test_PathFinding : MonoBehaviour
{
    public enum GridType
    {
        Path,
        Obstacle,
        Start,
        End
    }

    public class Grid
    {
        public Tile tile;
        public GridType type;
        public int x;
        public int y;

        public Vector3Int posInternal;

        private Vector2Int pos;
        public Vector2Int Pos
        {
            get
            {
                if (pos == default) pos = new Vector2Int(x, y);
                return pos;
            }
        }

        public Grid ParentGrid { get; set; }

        public Grid(Tile tile, GridType type, Vector3Int posInternal, int x, int y)
        {
            this.tile = tile;
            this.type = type;
            this.posInternal = posInternal;
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }

    public Tilemap tilemap;
    public TileBase visitedTile;
    public TileBase finalTile;

    [Range(1, 100)]
    public float speed;

    private Grid startGrid;
    private Grid endGrid;

    private Grid[,] originGridMap;
    private Grid[,] gridMap;

    private List<Grid> path = new List<Grid>();
    private List<Grid> finalPath = new List<Grid>();
    private List<Grid> visited = new List<Grid>();

    private BoolWrapper isFinish = new BoolWrapper(false);

    private void Awake()
    {
        //**注意**
        //如果进行过擦除Tile操作，这不会自动重计算Bound，需要：
        //1.点击Tilemap的三点中的Compress Tilemap Bounds
        //2.tilemap.CompressBounds();
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
                gridMap[x, y] = new Grid(tile, type, position, x, y);

                if (type == GridType.Start) startGrid = gridMap[x, y];
                else if (type == GridType.End) endGrid = gridMap[x, y];
            }
        }
        originGridMap = gridMap;//保存原始状态，在结束后恢复
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DFS();
        }
    }

    private void OnApplicationQuit()
    {
        ResetColor();
    }

    private void DFS()
    {
        MCoroutineManager.Instance.DelayNoRecord(() =>
        {
            StartCoroutine(DFSTraverse(startGrid, null));
        }, 1.0f);

        MCoroutineManager.Instance.WaitNoRecord(() =>
        {
            foreach (var grid in finalPath)
            {
                tilemap.SetTile(grid.posInternal, finalTile);
            }
        }, isFinish);

    }
    private IEnumerator DFSTraverse(Grid grid, Grid parentGrid)
    {
        yield return new WaitForSeconds(0.25f / speed);

        if (isFinish.Value) yield break;//已完成就回退
        if (grid == null) yield break;//未获取到grid，即撞墙或出界了
        if (parentGrid != null && parentGrid.ParentGrid == grid) yield break;//回头了，不应该进行
        if (visited.Contains(grid)) yield break;

        //完成条件
        if (grid.Pos == endGrid.Pos)
        {
            isFinish.Value = true;
            finalPath = new List<Grid>(path);//复制存储
            yield break;
        }

        grid.ParentGrid = parentGrid;
        path.Add(grid);
        visited.Add(grid);
        if (grid.type == GridType.Path)
        {
            tilemap.SetTile(grid.posInternal, visitedTile);
        }

        yield return DFSTraverse(GetGrid(grid, 1, 0), grid);
        yield return DFSTraverse(GetGrid(grid, 0, 1), grid);
        yield return DFSTraverse(GetGrid(grid, -1, 0), grid);
        yield return DFSTraverse(GetGrid(grid, 0, -1), grid);

        path.Remove(grid);
    }

    private Grid GetGrid(Grid grid, int xOffset, int yOffset)
    {
        if (grid.type == GridType.Obstacle) return null;

        int x = grid.x + xOffset, y = grid.y + yOffset;
        if(x < 0 || y < 0 || x >= gridMap.GetLength(0) || y >= gridMap.GetLength(1)) return null;

        return gridMap[grid.x + xOffset, grid.y + yOffset];
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
