using UnityEngine;
using UnityEngine.Tilemaps;

public class Map
{
    public int id;

    public Tilemap tilemap;

    public Grid[,] originGridMap;
    public Grid[,] gridMap;

    public Map(Tilemap tilemap, int id)
    {
        this.id = id;
        this.tilemap = tilemap;
    }

    public override bool Equals(object obj)
    {
        if (obj is Map other)
        {
            return tilemap.Equals(other.tilemap);
        }
        return false;
    }
    public override int GetHashCode()
    {
        return tilemap.GetHashCode();
    }

    public Grid GetGrid(Vector3Int pos)
    {
        for (int i = 0; i < originGridMap.GetLength(0); i++)
        {
            for (int j = 0; j < originGridMap.GetLength(1); j++)
            {
                if (originGridMap[i, j].posInternal == pos)
                {
                    return originGridMap[i, j];
                }
            }
        }
        return null;
    }
}