using MFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 关卡信息
/// </summary>
[Serializable]
public class GameLevel
{
    //---固有信息---
    public bool locked;//既是固有信息又是可变信息

    public string scene;
    public List<string> names;//0
    public string Name 
    {
        get
        {
            if (MLocalizationManager.Instance.CurrentLanguage == SupportLanguage.ENGLISH)
            {
                return names[0];
            }
            else if (MLocalizationManager.Instance.CurrentLanguage == SupportLanguage.CHINESE)
            {
                return names[1];
            }
            return null;
        }
    }
    public Sprite previewImage;

    //---可变信息---
    public int coins { get; set; }
    public float time { get; set; }
    public bool[] stars { get; set; } = new bool[StarsPerLevel];

    public static readonly int StarsPerLevel = 3;

    /// <summary>
    /// 根据选择LevelData，加载信息
    /// </summary>
    public virtual void LoadState(LevelData data)
    {
        locked = data.locked;
        coins = data.coins;
        time = data.time;
        stars = data.stars;
    }

    public virtual LevelData ToData()
    {
        return new LevelData()
        {
            locked = this.locked,
            coins = this.coins,
            time = this.time,
            stars = this.stars
        };
    }

    public static string FormattedTime(float time)
    {
        var minutes = Mathf.FloorToInt(time / 60f);
        var seconds = Mathf.FloorToInt(time % 60f);
        var milliseconds = Mathf.FloorToInt((time * 100f) % 100f);
        return minutes.ToString("0") + "'" + seconds.ToString("00") + "\"" + milliseconds.ToString("00");
    }
}