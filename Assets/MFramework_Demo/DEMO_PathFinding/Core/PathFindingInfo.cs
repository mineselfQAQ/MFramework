using MFramework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFindingInfo : ComponentSingleton<PathFindingInfo>
{
    public Tilemap[] Tilemaps;
    public TileBase VisitedTile;
    public TileBase FinalTile;

    [Range(1, 100)]
    public float Speed = 1.0f;

    protected override void Awake()
    {
        base.Awake();

        if (VisitedTile == null || FinalTile == null)
        {
            MLog.Print($"{typeof(PathFindingInfo)}：有参数未传入，请重试", MLogType.Error);
        }
    }
}
