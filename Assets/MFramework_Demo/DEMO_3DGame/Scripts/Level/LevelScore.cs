using MFramework;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 当前关卡分数信息收集
/// </summary>
public class LevelScore : ComponentSingleton<LevelScore>
{
    public UnityEvent<int> OnCoinsSet;
    public UnityEvent<bool[]> OnStarsSet;
    public UnityEvent OnScoreLoaded;

    /// <summary>
    /// 硬币收集状态(本地)
    /// </summary>
    protected int m_coins;
    /// <summary>
    /// 星收集状态(本地)
    /// </summary>
    protected bool[] m_stars = new bool[GameLevel.StarsPerLevel];

    public int coins
    {
        get { return m_coins; }

        set
        {
            m_coins = value;
            OnCoinsSet?.Invoke(m_coins);
        }
    }

    public bool[] stars => (bool[])m_stars.Clone();

    public float time { get; protected set; }

    public bool stopTime { get; set; } = true;

    protected Game m_game;
    /// <summary>
    /// //Game中当前GameLevel(引用)
    /// </summary>
    protected GameLevel m_level;

    public virtual void Reset()
    {
        time = 0;
        coins = 0;

        if (m_level != null)
        {
            m_stars = (bool[])m_level.stars.Clone();
        }
    }

    protected virtual void Start()
    {
        m_game = Game.Instance;
        m_level = m_game?.GetCurrentLevel();

        if (m_level != null)
        {
            m_stars = (bool[])m_level.stars.Clone();
        }

        //延迟调用，保证OnScoreLoaded回调添加完毕
        MCoroutineManager.Instance.DelayNoRecord(() =>
        {
            OnScoreLoaded?.Invoke();
        }, 0.1f);
    }

    protected virtual void Update()
    {
        if (!stopTime)
        {
            time += Time.deltaTime;
        }
    }

    /// <summary>
    /// 收集星
    /// </summary>
    public virtual void CollectStar(int index)
    {
        m_stars[index] = true;//更新星状态(本地)
        OnStarsSet?.Invoke(m_stars);
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    public virtual void FullSave()
    {
        if (m_level != null)
        {
            //时间
            if (m_level.time == 0 || time < m_level.time)
            {
                m_level.time = time;
            }
            //金币
            if (coins > m_level.coins)
            {
                m_level.coins = coins;
            }
            //星数
            m_level.stars = (bool[])stars.Clone();

            m_game.SaveState();
        }
    }

    /// <summary>
    /// 死亡时更新数据
    /// </summary>
    public virtual void GameOverSave()
    {
        if (m_level != null)
        {
            m_game.SaveState();//更新命数
        }
    }
}