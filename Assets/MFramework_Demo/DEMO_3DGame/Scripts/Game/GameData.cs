using System;
using System.Linq;

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
            retries = Game.Instance.initialRetries,
            createdTime = DateTime.UtcNow.ToString(),
            updatedTime = DateTime.UtcNow.ToString(),

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