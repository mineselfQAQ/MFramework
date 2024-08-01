using System.Linq;
using System;

/// <summary>
/// 关卡数据(JSON文件)
/// </summary>
[Serializable]
public class LevelData
{
    public bool locked;//关卡解锁状态
    public int coins;//收集金币最高记录
    public float time;//最佳时间
    public bool[] stars = new bool[GameLevel.StarsPerLevel];//星星收集状态

    /// <summary>
    /// 该关卡已收集星星数量
    /// </summary>
    public int CollectedStars()
    {
        return stars.Where((star) => star).Count();
    }
}