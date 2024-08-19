using UnityEngine.UI;

public class LevelCardWidget : LevelCardWidgetBase
{
    protected bool m_locked;

    public string scene { get; set; }

    public bool locked
    {
        get { return m_locked; }

        set
        {
            m_locked = value;
            m_PlayBtn_MButton.interactable = !m_locked;
        }
    }

    public void Init(GameLevel level)
    {
        Fill(level);
    }
    public void Refresh(GameLevel level)
    {
        Fill(level);
    }

    protected override void OnClicked(Button button)
    {
        if (button == m_PlayBtn_MButton)
        {
            Play();
        }
    }

    public virtual void Play()
    {
        GameLoader.Instance.Load(scene, () =>
        {
            UIController.Instance.bottomRoot.ClosePanel(UIController.levelSelectPanelName);
        });
    }

    public virtual void Fill(GameLevel level)
    {
        if (level != null)
        {
            locked = level.locked;
            scene = level.scene;
            m_LevelName_MText.text = level.Name;
            m_BestTime_MText.text = GameLevel.FormattedTime(level.time);
            m_Coins_MText.text = level.coins.ToString("000");
            m_LevelImage_MImage.sprite = level.previewImage;

            m_Star0_MImage.enabled = level.stars[0];
            m_Star1_MImage.enabled = level.stars[1];
            m_Star2_MImage.enabled = level.stars[2];
        }
    }
}