using MFramework;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine;

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
    /// ����ѡ��GameData��������Ϣ
    /// </summary>
    public virtual void LoadState(int index, GameData data)
    {
        dataIndex = index;
        retries = data.retries;
        createdTime = DateTime.Parse(data.createdTime);
        updatedTime = DateTime.Parse(data.updatedTime);

        for (int i = 0; i < data.levels.Length; i++)
        {
            levels[i].LoadState(data.levels[i]);//����Ϊ��ǰ�浵�ؿ���Ϣ
        }
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

    //TODO:...
}
