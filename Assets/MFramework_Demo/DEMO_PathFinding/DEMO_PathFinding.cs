using MFramework;
using MFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DEMO_PathFinding : MonoBehaviour
{
    public MText text;

    private List<Map> infos = new List<Map>();
    private Map preMap;
    private Map curMap;

    private Tilemap startEndMap;
    /// <summary>
    /// Key---cellPos Value---Tuple<WorldPos, MarkType>
    /// </summary>
    private Dictionary<Vector3Int, Tuple<Vector3, MarkType>> markTypeDic 
        = new Dictionary<Vector3Int, Tuple<Vector3, MarkType>>();

    private PathFindingSystem pathFindingSystem;
    private IPathFindingStrategy DFSStrategy;
    private IPathFindingStrategy BFSStrategy;
    private IPathFindingStrategy GreedyStrategy;
    private IPathFindingStrategy DijkstraStrategy;
    private IPathFindingStrategy AStarStrategy;

    private Dictionary<KeyCode, Action> strategyMap;
    private Dictionary<KeyCode, int> tileMapMap;
    private KeyCode directionKey;
    private Dictionary<GridType, int> gridTypeCostMap;

    private void Awake()
    {
        //输入映射
        strategyMap = new Dictionary<KeyCode, Action>
        {
            { KeyCode.Q, ()=>{ pathFindingSystem.SetStrategy(DFSStrategy); } },
            { KeyCode.W, ()=>{ pathFindingSystem.SetStrategy(BFSStrategy); } },
            { KeyCode.E, ()=>{ pathFindingSystem.SetStrategy(GreedyStrategy); } },
            { KeyCode.R, ()=>{ pathFindingSystem.SetStrategy(DijkstraStrategy); } },
            { KeyCode.T, ()=>{ pathFindingSystem.SetStrategy(AStarStrategy); } }
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
        directionKey = KeyCode.Z;

        //**注意**
        //如果进行过擦除Tile操作，这不会自动重计算Bound，需要：
        //1.点击Tilemap的三点中的Compress Tilemap Bounds
        //2.tilemap.CompressBounds();
        startEndMap = PathFindingInfo.Instance.StartEndMap;
        startEndMap.CompressBounds();

        var tilemaps = PathFindingInfo.Instance.Tilemaps;
        int id = 1;//id从1记录

        //遍历所有tilemap，收集信息
        foreach (var tilemap in tilemaps)
        {
            Map map = new Map(tilemap, id);
            tilemap.CompressBounds();

            BoundsInt bounds = tilemap.cellBounds;
            map.originGridMap = new Grid[bounds.size.x, bounds.size.y];
            map.gridMap = new Grid[bounds.size.x, bounds.size.y];

            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    int xPos = bounds.xMin + x;
                    int yPos = bounds.yMin + y;
                    Vector3Int position = new Vector3Int(xPos, yPos, 0);

                    //基础信息填写
                    Tile tile = tilemap.GetTile<Tile>(position);
                    GridType type = Enum.Parse<GridType>(tile.name);
                    int cost = gridTypeCostMap[type];//映射中获取权重
                    map.gridMap[x, y] = new Grid(tile, type, map.gridMap, position, x, y, cost);
                }
            }
            map.originGridMap = map.gridMap;//保存原始状态，在结束后恢复

            infos.Add(map);
            id++;
        }

        //寻路算法策略初始化
        DFSStrategy = new DFSPathFindingStrategy();
        BFSStrategy = new BFSPathFindingStrategy();
        GreedyStrategy = new GreedyBFSPathFindingStrategy();
        DijkstraStrategy = new DijkstraStrategy();
        AStarStrategy = new AStarPathFindingStrategy();
        pathFindingSystem = new PathFindingSystem(AStarStrategy);//默认A*

        //设置默认显示
        curMap = infos[0];
        foreach (var info in infos)
        {
            if (info.Equals(curMap))
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
        //选择Start/End(进行中禁止切换)
        if (Input.GetMouseButtonDown(0) && pathFindingSystem.IsFinish)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;//确保z轴为0(2D)

            Vector3Int cellPos = startEndMap.WorldToCell(worldPos);
            MarkType type = PathFindingUtility.SetStartEndMark(startEndMap, cellPos, curMap);
            if (type == MarkType.Error) return;

            //注意worldPos是根据具体点击位置的，所以需要通过cellPos判断是否点击过
            if (!markTypeDic.ContainsKey(cellPos))
            {
                markTypeDic.Add(cellPos, new(worldPos, type));
            }
            else
            {
                markTypeDic[cellPos] = new(worldPos, type);
            }
        }

        //执行
        if (Input.GetKeyDown(KeyCode.Space))
        {
             GetStartEndWorldPos(out var startWorldPos, out var endWorldPos);
            //非法情况
            if (startWorldPos == new Vector3(0, 0, -999) ||
                endWorldPos == new Vector3(0, 0, -999))
                return;

            var startPos = curMap.tilemap.WorldToCell(startWorldPos);
            var endPos = curMap.tilemap.WorldToCell(endWorldPos);
            Grid startGrid = curMap.GetGrid(startPos);
            Grid endGrid = curMap.GetGrid(endPos);

            ResetColor(curMap);
            ResetCost(curMap);
            pathFindingSystem.Execute(curMap.tilemap, startGrid, endGrid);
        }

        //策略选择
        foreach (var pair in strategyMap)
        {
            //进行中禁止切换
            if (Input.GetKeyDown(pair.Key) && pathFindingSystem.IsFinish)
            {
                pair.Value.Invoke();
                RefreshText();
            }
        }

        //Tilemap选择                                                 
        foreach (var pair in tileMapMap)
        {
            if (pair.Value >= infos.Count) continue;

            //进行中禁止切换
            if (Input.GetKeyDown(pair.Key) && pathFindingSystem.IsFinish)
            {
                ChangeTilemap(pair.Value);
            }
        }

        if (Input.GetKeyDown(directionKey) && pathFindingSystem.IsFinish)
        {
            ChangeDirection();
            RefreshText();
        }
    }

    private void GetStartEndWorldPos(out Vector3Int startPos, out Vector3Int endPos)
    {
        startPos = new Vector3Int(0, 0, -999);
        endPos = new Vector3Int(0, 0, -999);

        var startGroup = markTypeDic.Where(pair => pair.Value.Item2 == MarkType.Start).ToArray();
        var endGroup = markTypeDic.Where(pair => pair.Value.Item2 == MarkType.End).ToArray();
        if (startGroup.Length != 1 || endGroup.Length != 1)
        {
            MLog.Print($"Start/End不符合要求，请重试", MLogType.Warning);
            return;
        }

        startPos = startGroup[0].Key;
        endPos = endGroup[0].Key;
    }

    private void OnApplicationQuit()
    {
        foreach (var info in infos)
        {
            ResetColor(info);
        }
    }

    /// <summary>
    /// 由于SetTile会永久更改，需要复原
    /// </summary>
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
        preMap = curMap;
        curMap = infos[index];

        if (!preMap.Equals(curMap))
        {
            preMap.tilemap.gameObject.SetActive(false);
            curMap.tilemap.gameObject.SetActive(true);
            RefreshMarkType();
            RefreshText();
        }
        ResetColor(preMap);//无论如何，切换一定进行一次刷新
    }
    private void ChangeDirection()
    {
        if (PathFindingInfo.Instance.dir == PathDir.Dir4)
        {
            PathFindingInfo.Instance.dir = PathDir.Dir8;
        }
        else if (PathFindingInfo.Instance.dir == PathDir.Dir8)
        {
            PathFindingInfo.Instance.dir = PathDir.Dir4;
        }
    }

    private void RefreshText()
    {
        text.text = $"Current:{pathFindingSystem.Strategy}(ID-{curMap.id}|{PathFindingInfo.Instance.dir})";
    }

    private void RefreshMarkType()
    {
        var startGroup = markTypeDic.Where(pair => pair.Value.Item2 == MarkType.Start).ToArray();
        var endGroup = markTypeDic.Where(pair => pair.Value.Item2 == MarkType.End).ToArray();
        foreach (var start in startGroup)
        {
            PathFindingUtility.SetNull(startEndMap, start.Key);
        }
        foreach (var end in endGroup)
        {
            PathFindingUtility.SetNull(startEndMap, end.Key);
        }

        markTypeDic.Clear();
    }
}