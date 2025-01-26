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

    [Range(1, 100)]
    public int Speed = 1;
    [Range(2, 10)]
    public int BarrierCost = 3;

    protected override void Awake()
    {
        base.Awake();

        if (VisitedTile == null || FinalTile == null)
        {
            MLog.Print($"{typeof(PathFindingInfo)}���в���δ���룬������", MLogType.Error);
        }
    }
}
