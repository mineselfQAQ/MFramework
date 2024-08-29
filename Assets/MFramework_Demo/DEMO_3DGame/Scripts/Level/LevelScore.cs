using MFramework;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ��ǰ�ؿ�������Ϣ�ռ�
/// </summary>
public class LevelScore : ComponentSingleton<LevelScore>
{
    public UnityEvent<int> OnCoinsSet;
    public UnityEvent<bool[]> OnStarsSet;
    public UnityEvent OnScoreLoaded;

    /// <summary>
    /// Ӳ���ռ�״̬(����)
    /// </summary>
    protected int m_coins;
    /// <summary>
    /// ���ռ�״̬(����)
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
    /// //Game�е�ǰGameLevel(����)
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

        //�ӳٵ��ã���֤OnScoreLoaded�ص�������
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
    /// �ռ���
    /// </summary>
    public virtual void CollectStar(int index)
    {
        m_stars[index] = true;//������״̬(����)
        OnStarsSet?.Invoke(m_stars);
    }

    /// <summary>
    /// ��������
    /// </summary>
    public virtual void FullSave()
    {
        if (m_level != null)
        {
            //ʱ��
            if (m_level.time == 0 || time < m_level.time)
            {
                m_level.time = time;
            }
            //���
            if (coins > m_level.coins)
            {
                m_level.coins = coins;
            }
            //����
            m_level.stars = (bool[])stars.Clone();

            m_game.SaveState();
        }
    }

    /// <summary>
    /// ����ʱ��������
    /// </summary>
    public virtual void GameOverSave()
    {
        if (m_level != null)
        {
            m_game.SaveState();//��������
        }
    }
}