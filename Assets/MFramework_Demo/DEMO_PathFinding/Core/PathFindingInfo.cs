using MFramework;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum PathDir
{
    Dir4,//上下左右
    Dir8//上下左右+斜方向
}

public class PathFindingInfo : ComponentSingleton<PathFindingInfo>
{
    public Tilemap StartEndMap;//叠加Start/End
    public Tilemap[] Tilemaps;

    public TileBase VisitedTile;
    public TileBase VisitedBarrier;
    public TileBase FinalTile;
    public TileBase StartMark;
    public TileBase EndMark;

    [Range(1, 100)]
    public int Speed = 1;
    [Range(2, 10)]
    public int BarrierCost = 3;

    [NonSerialized]
    public PathDir dir = PathDir.Dir4; 

    public const string startGridName = "Start";
    public const string endGridName = "End";

    protected override void Awake()
    {
        base.Awake();

        if (StartEndMap == null || Tilemaps == null ||
            VisitedTile == null || VisitedBarrier == null || FinalTile == null ||
            StartMark == null || EndMark == null)
        {
            MLog.Print($"{typeof(PathFindingInfo)}：有参数未传入，请重试", MLogType.Error);
        }
    }
}
