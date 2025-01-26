using MFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DEMO_PathFinding : MonoBehaviour
{
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
    }

    public MText text;

    private List<Map> infos = new List<Map>();
    private Map preInfo;
    private Map curInfo;

    private Vector2Int startGridPos;
    private Vector2Int endGridPos;

    public PathFindingSystem pathFindingSystem;
    public IPathFindingStrategy DFSStrategy;
    public IPathFindingStrategy BFSStrategy;
    public IPathFindingStrategy GreedyStrategy;
    public IPathFindingStrategy DijkstraStrategy;

    private Dictionary<KeyCode, Action> strategyMap;
    private Dictionary<KeyCode, int> tileMapMap;
    private Dictionary<GridType, int> gridTypeCostMap;

    private const string startGridName = "Start";
    private const string endGridName = "End";

    private void Awake()
    {
        //����ӳ��
        strategyMap = new Dictionary<KeyCode, Action>
        {
            { KeyCode.Q, ()=>{ pathFindingSystem.SetStrategy(DFSStrategy); } },
            { KeyCode.W, ()=>{ pathFindingSystem.SetStrategy(BFSStrategy); } },
            { KeyCode.E, ()=>{ pathFindingSystem.SetStrategy(GreedyStrategy); } },
            { KeyCode.R, ()=>{ pathFindingSystem.SetStrategy(DijkstraStrategy); } }
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
        gridTypeCostMap = new Dictionary<GridType, int>()
        {
            { GridType.Path, 1 },
            { GridType.Obstacle, -1 },
            { GridType.Barrier, PathFindingInfo.Instance.BarrierCost },
        };

        //**ע��**
        //������й�����Tile�������ⲻ���Զ��ؼ���Bound����Ҫ��
        //1.���Tilemap�������е�Compress Tilemap Bounds
        //2.tilemap.CompressBounds();
        var startEndMap = PathFindingInfo.Instance.StartEndMap;
        var tilemaps = PathFindingInfo.Instance.Tilemaps;
        int id = 1;//id��1��¼

        var tempTilemap = tilemaps[0];
        BoundsInt bounds = tempTilemap.cellBounds;//����tilemap��boundsһ��

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                int xPos = bounds.xMin + x;
                int yPos = bounds.yMin + y;
                Vector3Int position = new Vector3Int(xPos, yPos, 0);

                //����Ƿ�Ϊ��ʼ/����Grid
                Tile tile = startEndMap.GetTile<Tile>(position);
                if (tile != null && tile.name == startGridName)
                {
                    startGridPos = new Vector2Int(x, y);
                }
                if (tile != null && tile.name == endGridName)
                {
                    endGridPos = new Vector2Int(x, y);
                }
            }
        }

        foreach (var tilemap in tilemaps)
        {
            Map info = new Map(tilemap, id);

            tilemap.CompressBounds();

            info.originGridMap = new Grid[bounds.size.x, bounds.size.y];
            info.gridMap = new Grid[bounds.size.x, bounds.size.y];

            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    int xPos = bounds.xMin + x;
                    int yPos = bounds.yMin + y;
                    Vector3Int position = new Vector3Int(xPos, yPos, 0);

                    //������Ϣ��д
                    Tile tile = tilemap.GetTile<Tile>(position);
                    GridType type = Enum.Parse<GridType>(tile.name);
                    int cost = gridTypeCostMap[type];//ӳ���л�ȡȨ��
                    info.gridMap[x, y] = new Grid(tile, type, info.gridMap, position, x, y, cost);

                    if (info.gridMap[x, y].Pos == startGridPos)
                    {
                        info.gridMap[x, y].SetStartGrid();
                    }
                    if (info.gridMap[x, y].Pos == endGridPos) 
                    {
                        info.gridMap[x, y].SetEndGrid();
                    }
                }
            }
            info.originGridMap = info.gridMap;//����ԭʼ״̬���ڽ�����ָ�


            infos.Add(info);
            id++;
        }

        //Ѱ·�㷨���Գ�ʼ��
        DFSStrategy = new DFSPathFindingStrategy();
        BFSStrategy = new BFSPathFindingStrategy();
        GreedyStrategy = new GreedyBFSPathFindingStrategy();
        DijkstraStrategy = new DijkstraStrategy();
        pathFindingSystem = new PathFindingSystem(DijkstraStrategy);

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
            ResetCost(curInfo);
            pathFindingSystem.Execute(curInfo.tilemap, startGrid, endGrid);
        }

        //����ѡ��
        foreach (var pair in strategyMap)
        {
            //�����н�ֹ�л�
            if (Input.GetKeyDown(pair.Key) && pathFindingSystem.IsFinish)
            {
                pair.Value.Invoke();
                RefreshText();
            }
        }

        //Tilemapѡ��                                                 
        foreach (var pair in tileMapMap)
        {
            if (pair.Value >= infos.Count) continue;

            //�����н�ֹ�л�
            if (Input.GetKeyDown(pair.Key) && pathFindingSystem.IsFinish)
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

    private void ResetColor(Map info)
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
    private void ResetCost(Map info)
    {
        for (int x = 0; x < info.originGridMap.GetLength(0); x++)
        {
            for (int y = 0; y < info.originGridMap.GetLength(1); y++)
            {
                var grid = info.originGridMap[x, y];
                grid.ResetCost();
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
        text.text = $"Current:{pathFindingSystem.Strategy}(ID-{curInfo.id})";
    }
}
