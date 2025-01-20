using UnityEngine;
using UnityEngine.Tilemaps;

public class Test_PathFinding : MonoBehaviour
{
    public class Grid
    {
        public Tile tile;
        public int x;
        public int y;

        public Grid(Tile tile, int x, int y)
        {
            this.tile = tile;
            this.x = x;
            this.y = y;
        }
    }

    public Tilemap tilemap;

    private void Start()
    {
        //**注意**
        //如果进行过擦除Tile操作，这不会自动重计算Bound，需要：
        //1.点击Tilemap的三点中的Compress Tilemap Bounds
        //2.tilemap.CompressBounds();
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;

        Grid[,] tileArray = new Grid[bounds.size.x, bounds.size.y];

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                int xPos = bounds.xMin + x;
                int yPos = bounds.yMin + y;

                Vector3Int position = new Vector3Int(x, y, 0);
                Tile tile = tilemap.GetTile<Tile>(position);
                tileArray[x, y] = new Grid(tile, x, y);
            }
        }
    }
}
