using System;
using UnityEngine;

/// <summary>
/// 关卡信息
/// </summary>
[Serializable]
public class GameLevel
{
    public bool locked;
    public string scene;
    public string name;
    public Sprite image;

    public int coins { get; set; }
    public float time { get; set; }
    public bool[] stars { get; set; } = new bool[StarsPerLevel];

    public static readonly int StarsPerLevel = 3;

    public virtual void LoadState(LevelData data)
    {
        locked = data.locked;
        coins = data.coins;
        time = data.time;
        stars = data.stars;
    }

    public static string FormattedTime(float time)
    {
        var minutes = Mathf.FloorToInt(time / 60f);
        var seconds = Mathf.FloorToInt(time % 60f);
        var milliseconds = Mathf.FloorToInt((time * 100f) % 100f);
        return minutes.ToString("0") + "'" + seconds.ToString("00") + "\"" + milliseconds.ToString("00");
    }
}