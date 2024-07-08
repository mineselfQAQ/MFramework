using System.Linq;
using System;

/// <summary>
/// 关卡数据
/// </summary>
[Serializable]
public class LevelData
{
    public bool locked;
    public int coins;
    public float time;
    public bool[] stars = new bool[GameLevel.StarsPerLevel];

    public int CollectedStars()
    {
        return stars.Where((star) => star).Count();
    }
}