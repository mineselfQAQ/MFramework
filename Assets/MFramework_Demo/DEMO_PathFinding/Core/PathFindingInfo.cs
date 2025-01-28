using MFramework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFindingInfo : ComponentSingleton<PathFindingInfo>
{
    public Tilemap StartEndMap;//����Start/End
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

    public const string startGridName = "Start";
    public const string endGridName = "End";

    protected override void Awake()
    {
        base.Awake();

        if (StartEndMap == null || Tilemaps == null ||
            VisitedTile == null || VisitedBarrier == null || FinalTile == null ||
            StartMark == null || EndMark == null)
        {
            MLog.Print($"{typeof(PathFindingInfo)}���в���δ���룬������", MLogType.Error);
        }
    }
}
