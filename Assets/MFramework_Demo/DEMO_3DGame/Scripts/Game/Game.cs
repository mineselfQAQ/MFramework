using MFramework;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine;

public class Game : ComponentSingleton<Game>
{
    [Header("GameInfo")]
    public int initialRetries = 3;//��ʼ����
    public List<GameLevel> levels;//�ؼ�---�ؿ�������Ϣ

    [Space(10)][Header("Event")]
    public UnityEvent<int> OnRetriesSet;
    public UnityEvent OnSavingRequested;

    //��ǰѡ��浵��Ϣ
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
        Retries = initialRetries;
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
            levels[i].LoadState(data.levels[i]);
        }
    }
}
