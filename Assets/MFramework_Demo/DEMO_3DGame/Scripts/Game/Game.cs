using MFramework;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine;
using System.Linq;

/// <summary>
/// ��Ϸ��Ϣ������
/// </summary>
[RequireComponent(typeof(GameLoader))]
[RequireComponent(typeof(GameSaver))]
public class Game : ComponentSingleton<Game>
{
    [Header("GameInfo")]
    public int initRetries = 3;//��ʼ����
    public List<GameLevel> levels;//�ؿ���Ϣ(���������ɴ���Ĭ�����ã���LoadState()��ɻ�ȡ��ǰ��ϸ��Ϣ)

    [Space(10)][Header("Event")]
    public UnityEvent<int> OnRetriesSet;
    public UnityEvent OnSavingRequested;

    //˽�б���Ϊ��ǰѡ��浵��Ϣ(ͨ��LoadState()��ȡ)
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
    /// ����ѡ��GameData��������Ϣ
    /// </summary>
    public virtual void LoadState(int index, GameData data)
    {
        m_dataIndex = index;
        m_retries = data.retries;
        m_createdTime = DateTime.Parse(data.createdTime);
        m_updatedTime = DateTime.Parse(data.updatedTime);

        for (int i = 0; i < data.levels.Length; i++)
        {
            levels[i].LoadState(data.levels[i]);//����Ϊ��ǰ�浵�ؿ���Ϣ
        }
    }

    public virtual void SaveState()
    {
        GameSaver.Instance.Save(ToData(), m_dataIndex);
        OnSavingRequested?.Invoke();
    }

    /// <summary>
    /// �������
    /// </summary>
    public static void LockCursor(bool value = true)
    {
#if UNITY_STANDALONE || UNITY_WEBGL
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
#endif
    }

    /// <summary>
    /// ������һ�ؿ�(ͨ�غ�)
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
