using UnityEngine;
using UnityEngine.Tilemaps;

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
    public Grid[,] gridMap;//所属GridMap
    public int x { get; private set; }
    public int y { get; private set; }

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

    public Grid(Tile tile, GridType type, Grid[,] gridMap, Vector3Int posInternal, int x, int y)
    {
        this.tile = tile;
        this.type = type;
        this.gridMap = gridMap;
        this.posInternal = posInternal;
        this.x = x;
        this.y = y;
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

    public static Grid GetGrid(Grid[,] gridMap, Grid grid, int xOffset, int yOffset)
    {
        if (grid.type == GridType.Obstacle) return null;

        int newX = grid.x + xOffset, newY = grid.y + yOffset;
        if (newX < 0 || newY < 0 || newX >= gridMap.GetLength(0) || newY >= gridMap.GetLength(1)) return null;

        return gridMap[newX, newY];
    }
}
