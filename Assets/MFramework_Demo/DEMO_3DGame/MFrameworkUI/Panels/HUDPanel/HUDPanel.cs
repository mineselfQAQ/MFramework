using MFramework;
using UnityEngine;

public class HUDPanel : HUDPanelBase
{
    protected Game m_game;
    protected LevelScore m_score;
    protected UIController m_controller;

    protected Player m_player;

    protected float timerStep;
    protected static float timerRefreshRate = 0.1f;

    public override void Init()
    {
        //执行ResetPanel()代替
    }

    /// <summary>
    /// 重置HUDPanel
    /// </summary>
    public virtual void ResetPanel()
    {
        m_game = Game.Instance;
        m_score = LevelScore.Instance;
        m_player = Object.FindObjectOfType<Player>();
        m_controller = UIController.Instance;

        m_score.OnScoreLoaded.AddListener(() =>
        {
            m_score.OnCoinsSet.AddListener(UpdateCoins);
            m_score.OnStarsSet.AddListener(UpdateStars);
            m_game.OnRetriesSet.AddListener(UpdateRetries);
            m_player.health.onChange.AddListener(UpdateHealth);
            Refresh();
        });
    }

    public override void Update()
    {
        if (isOpen)
        {
            UpdateTimer();
        }
    }

    /// <summary>
    /// 初始刷新
    /// </summary>
    public virtual void Refresh()
    {
        UpdateCoins(m_score.coins);
        UpdateRetries(m_game.retries);
        UpdateHealth();
        UpdateStars(m_score.stars);
    }

    protected virtual void UpdateCoins(int value)
    {
        m_CoinNum_MText.text = value.ToString(m_controller.HUDCoinsFormat);
    }
    protected virtual void UpdateRetries(int value)
    {
        m_RetriesNum_MText.text = value.ToString(m_controller.HUDRetriesFormat);
    }
    protected virtual void UpdateHealth()
    {
        m_HeartNum_MText.text = m_player.health.current.ToString(m_controller.HUDHealthFormat);
    }
    protected virtual void UpdateStars(bool[] value)
    {
        m_Star0_MImage.enabled = value[0];
        m_Star1_MImage.enabled = value[1];
        m_Star2_MImage.enabled = value[2];
    }
    protected virtual void UpdateTimer()
    {
        timerStep += Time.deltaTime;

        if (timerStep >= timerRefreshRate)
        {
            m_Timer_MText.text = GameLevel.FormattedTime(m_score.time);
            timerStep = 0;
        }
    }

    protected override GameObject LoadPrefab(string prefabPath)
    {
        return ABUtitlity.LoadPanelSync(prefabPath);
    }
}