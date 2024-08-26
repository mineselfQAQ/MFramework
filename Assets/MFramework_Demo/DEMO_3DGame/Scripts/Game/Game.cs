using MFramework;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine;
using System.Linq;

/// <summary>
/// 游戏信息核心类
/// </summary>
[RequireComponent(typeof(GameLoader))]
[RequireComponent(typeof(GameSaver))]
public class Game : ComponentSingleton<Game>
{
    [Header("GameInfo")]
    public int initRetries = 3;//初始生命
    public List<GameLevel> levels;//关卡信息(公开变量可传入默认设置，在LoadState()后可获取当前详细信息)

    [Space(10)][Header("Event")]
    public UnityEvent<int> OnRetriesSet;
    public UnityEvent OnSavingRequested;

    //私有变量为当前选择存档信息(通过LoadState()读取)
    protected int m_retries;
    protected int m_dataIndex;
    protected DateTime m_createdTime;
    protected DateTime m_updatedTime;

    public int retries
    {
        get { return m_retries; }

        set
        {
            m_retries = value;
            OnRetriesSet?.Invoke(m_retries);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        retries = initRetries;
    }

    /// <summary>
    /// 根据选择GameData，加载信息
    /// </summary>
    public virtual void LoadState(int index, GameData data)
    {
        m_dataIndex = index;
        m_retries = data.retries;
        m_createdTime = DateTime.Parse(data.createdTime);
        m_updatedTime = DateTime.Parse(data.updatedTime);

        for (int i = 0; i < data.levels.Length; i++)
        {
            levels[i].LoadState(data.levels[i]);//更改为当前存档关卡信息
        }
    }

    public virtual void SaveState()
    {
        GameSaver.Instance.Save(ToData(), m_dataIndex);
        OnSavingRequested?.Invoke();
    }

    /// <summary>
    /// 锁定光标
    /// </summary>
    public static void LockCursor(bool value = true)
    {
#if UNITY_STANDALONE || UNITY_WEBGL
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
#endif
    }

    /// <summary>
    /// 解锁下一关卡(通关后)
    /// </summary>
    public virtual void UnlockNextLevel()
    {
        var index = GetCurrentLevelIndex() + 1;

        if (index >= 0 && index < levels.Count)
        {
            levels[index].locked = false;
        }
    }

    public virtual GameLevel GetCurrentLevel()
    {
        string scene = GameLoader.Instance.currentScene;
        return levels.Find((level) => level.scene == scene);
    }

    public virtual int GetCurrentLevelIndex()
    {
        var scene = GameLoader.Instance.currentScene;
        return levels.FindIndex((level) => level.scene == scene);
    }

    public virtual GameData ToData()
    {
        return new GameData()
        {
            retries = m_retries,
            levels = LevelsData(),
            createdTime = m_createdTime.ToString(),
            updatedTime = DateTime.UtcNow.ToString()
        };
    }

    public virtual LevelData[] LevelsData()
    {
        return levels.Select(level => level.ToData()).ToArray();
    }
}
