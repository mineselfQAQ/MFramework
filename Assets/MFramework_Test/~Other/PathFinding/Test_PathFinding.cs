using MFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test_PathFinding : MonoBehaviour
{
    public class TilemapInfo
    {
        public int id;
                
        public Tilemap tilemap;
        public Grid startGrid;
        public Grid endGrid;
                
        public Grid[,] originGridMap;
        public Grid[,] gridMap;
                
        public PathFindingSystem pfSystem;
        public IPathFindingStrategy DFSPathFindingStrategy;
        public IPathFindingStrategy BFSPathFindingStrategy;
        public IPathFindingStrategy GreedyBFSPathFindingStrategy;

        public TilemapInfo(Tilemap tilemap, int id)
        {
            this.id = id;
            this.tilemap = tilemap;
        }

        public override bool Equals(object obj)
        {
            if (obj is TilemapInfo other)
            {
                return tilemap.Equals(other.tilemap);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return tilemap.GetHashCode();
        }
    }

    public MText text;

    private List<TilemapInfo> infos = new List<TilemapInfo>();
    private TilemapInfo preInfo;
    private TilemapInfo curInfo;

    private Dictionary<KeyCode, Action> strategyMap;
    private Dictionary<KeyCode, int> tileMapMap;

    private void Awake()
    {
        //����ӳ��
        strategyMap = new Dictionary<KeyCode, Action>
        {
            { KeyCode.Q, ()=>{ curInfo.pfSystem.SetStrategy(curInfo.DFSPathFindingStrategy); } },
            { KeyCode.W, ()=>{ curInfo.pfSystem.SetStrategy(curInfo.BFSPathFindingStrategy); } },
            { KeyCode.E, ()=>{ curInfo.pfSystem.SetStrategy(curInfo.GreedyBFSPathFindingStrategy); } }
        };
        tileMapMap = new Dictionary<KeyCode, int>
        {
            { KeyCode.Alpha1, 0 },
            { KeyCode.Alpha2, 1 },
            { KeyCode.Alpha3, 2 },
            { KeyCode.Alpha4, 3 },
            { KeyCode.Alpha5, 4 },
            { KeyCode.Alpha6, 5 },
            { KeyCode.Alpha7, 6 },
            { KeyCode.Alpha8, 7 },
            { KeyCode.Alpha9, 8 },
            { KeyCode.Alpha0, 9 },
        };

        //**ע��**
        //������й�����Tile�������ⲻ���Զ��ؼ���Bound����Ҫ��
        //1.���Tilemap�������е�Compress Tilemap Bounds
        //2.tilemap.CompressBounds();
        var tilemaps = PathFindingInfo.Instance.Tilemaps;
        int id = 1;//id��1��¼
        foreach (var tilemap in tilemaps)
        {
            TilemapInfo info = new TilemapInfo(tilemap, id);

            tilemap.CompressBounds();
            BoundsInt bounds = tilemap.cellBounds;

            info.originGridMap = new Grid[bounds.size.x, bounds.size.y];
            info.gridMap = new Grid[bounds.size.x, bounds.size.y];

            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    int xPos = bounds.xMin + x;
                    int yPos = bounds.yMin + y;

                    Vector3Int position = new Vector3Int(xPos, yPos, 0);
                    Tile tile = tilemap.GetTile<Tile>(position);
                    GridType type = Enum.Parse<GridType>(tile.name);
                    info.gridMap[x, y] = new Grid(tile, type, info.gridMap, position, x, y);

                    if (type == GridType.Start) info.startGrid = info.gridMap[x, y];
                    else if (type == GridType.End) info.endGrid = info.gridMap[x, y];
                }
            }
            info.originGridMap = info.gridMap;//����ԭʼ״̬���ڽ�����ָ�

            //TODO�����Բ�Ӧ���ǹ̶����ԣ�Ӧ�ÿ���ͨ�����Ĺ��캯����Щ�����л�
            info.DFSPathFindingStrategy = new DFSPathFindingStrategy(tilemap, info.startGrid, info.endGrid);
            info.BFSPathFindingStrategy = new BFSPathFindingStrategy(tilemap, info.startGrid, info.endGrid);
            info.GreedyBFSPathFindingStrategy = new GreedyBFSPathFindingStrategy(tilemap, info.startGrid, info.endGrid);
            info.pfSystem = new PathFindingSystem(info.BFSPathFindingStrategy);

            infos.Add(info);
            id++;
        }

        //����Ĭ����ʾ
        curInfo = infos[0];
        foreach (var info in infos)
        {
            if (info.Equals(curInfo))
            {
                info.tilemap.gameObject.SetActive(true);
            }
            else
            {
                info.tilemap.gameObject.SetActive(false);
            }
        }
        RefreshText();
    }

    private void Update()
    {
        //ִ��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetColor(curInfo);
            curInfo.pfSystem.ExecutePathfinding();
        }

        //����ѡ��
        foreach (var pair in strategyMap)
        {
            if (Input.GetKeyDown(pair.Key))
            {
                pair.Value.Invoke();
                RefreshText();
            }
        }

        //Tilemapѡ��
        foreach (var pair in tileMapMap)
        {
            if (Input.GetKeyDown(pair.Key))
            {
                ChangeTilemap(pair.Value);
                RefreshText();
            }
        }
    }

    private void OnApplicationQuit()
    {
        foreach (var info in infos)
        {
            ResetColor(info);
        }
    }

    private void ResetColor(TilemapInfo info)
    {
        for (int x = 0; x < info.originGridMap.GetLength(0); x++)
        {
            for (int y = 0; y < info.originGridMap.GetLength(1); y++)
            {
                var grid = info.originGridMap[x, y];
                info.tilemap.SetTile(grid.posInternal, grid.tile);
            }
        }
    }
    private void ChangeTilemap(int index)
    {
        preInfo = curInfo;
        curInfo = infos[index];

        if (!preInfo.Equals(curInfo))
        {
            preInfo.tilemap.gameObject.SetActive(false);
            curInfo.tilemap.gameObject.SetActive(true);
        }
        ResetColor(preInfo);//������Σ��л�һ������һ��ˢ��
    }

    private void RefreshText()
    {
        text.text = $"Current:{curInfo.pfSystem.Strategy}(ID-{curInfo.id})";
    }
}
