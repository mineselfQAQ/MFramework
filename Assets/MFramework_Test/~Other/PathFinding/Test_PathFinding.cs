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
        //**ע��**
        //������й�����Tile�������ⲻ���Զ��ؼ���Bound����Ҫ��
        //1.���Tilemap�������е�Compress Tilemap Bounds
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
