using UnityEngine;
using UnityEngine.Tilemaps;

public enum GridType
{
    Path,//一般路(消耗-1)
    Barrier,//阻挡物(缓慢通过，消耗-3)
    Obstacle,//障碍物(不可通过)
}

public class Grid
{
    public Tile tile;
    public GridType type;
    public Grid[,] gridMap;//所属GridMap
    public int x { get; private set; }
    public int y { get; private set; }

    public bool isStartGrid { get; private set; }
    public bool isEndGrid { get; private set; }

    public Vector3Int posInternal;

    public int cost;//默认为1，在Dijkstra算法/A*算法中需要消耗
    public int totalCost = 0;//起点到Grid的消耗

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

    public Grid(Tile tile, GridType type, Grid[,] gridMap, Vector3Int posInternal, int x, int y, int weight = 1)
    {
        this.tile = tile;
        this.type = type;
        this.gridMap = gridMap;
        this.posInternal = posInternal;
        this.x = x;
        this.y = y;

        this.cost = weight;
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }

    //===必须添加Equals()和GetHashCode()，否则比较(这里是字典是否Contain)会得到false
    public override bool Equals(object obj)
    {
        if (obj is Grid other)
        {
            return Pos.Equals(other.Pos);
        }
        return false;
    }
    public override int GetHashCode()
    {
        return Pos.GetHashCode();
    }

    public Grid GetGrid(int xOffset, int yOffset)
    {
        int newX = x + xOffset, newY = y + yOffset;
        if (newX < 0 || newY < 0 || newX >= gridMap.GetLength(0) || newY >= gridMap.GetLength(1)) return null;

        Grid res = gridMap[newX, newY];
        if (res.type == GridType.Obstacle) return null;

        return res;
    }

    public void ResetCost() => totalCost = 0;

    public void SetStartGrid() => isStartGrid = true;
    public void SetEndGrid() => isEndGrid = true;

    public static Grid GetGrid(Grid[,] gridMap, Grid grid, int xOffset, int yOffset)
    {
        if (grid.type == GridType.Obstacle) return null;

        int newX = grid.x + xOffset, newY = grid.y + yOffset;
        if (newX < 0 || newY < 0 || newX >= gridMap.GetLength(0) || newY >= gridMap.GetLength(1)) return null;

        return gridMap[newX, newY];
    }
}
