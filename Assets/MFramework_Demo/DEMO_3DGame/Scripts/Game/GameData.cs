using System;
using System.Linq;

/// <summary>
/// 游戏数据(JSON文件)
/// </summary>
public class GameData
{
    public int retries;
    public LevelData[] levels;
    public string createdTime;
    public string updatedTime;

    /// <summary>
    /// 创建GameData实例
    /// </summary>
    public static GameData Create()
    {
        return new GameData()
        {
            retries = Game.Instance.initRetries,
            //格式：21/8/2024 - 10:00
            createdTime = DateTime.UtcNow.ToString(),
            updatedTime = DateTime.UtcNow.ToString(),

            //通过Game中的GameLevel中的locked信息创建LevelData
            levels = Game.Instance.levels.Select((level) =>
            {
                return new LevelData()
                {
                    locked = level.locked//获取locked信息
                };
            }).ToArray()
        };
    }

    public virtual int TotalStars()
    {
        return levels.Aggregate(0, (acc, level) =>
        {
            var total = level.CollectedStars();
            return acc + total;
        });
    }
    public virtual int TotalCoins()
    {
        return levels.Aggregate(0, (acc, level) => acc + level.coins);
    }
}