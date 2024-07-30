using MFramework;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine;

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
    protected int retries;
    protected int dataIndex;
    protected DateTime createdTime;
    protected DateTime updatedTime;

    public int Retries
    {
        get { return retries; }

        set
        {
            retries = value;
            OnRetriesSet?.Invoke(retries);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        Retries = initRetries;
    }

    /// <summary>
    /// 根据选择GameData，加载信息
    /// </summary>
    public virtual void LoadState(int index, GameData data)
    {
        dataIndex = index;
        retries = data.retries;
        createdTime = DateTime.Parse(data.createdTime);
        updatedTime = DateTime.Parse(data.updatedTime);

        for (int i = 0; i < data.levels.Length; i++)
        {
            levels[i].LoadState(data.levels[i]);//更改为当前存档关卡信息
        }
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

    //TODO:...
}
