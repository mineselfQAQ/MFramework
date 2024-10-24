using MFramework.UI;
using UnityEngine;

public class LevelCell : MonoBehaviour
{
    protected bool m_locked;

    public bool locked
    {
        get { return m_locked; }

        set
        {
            m_locked = value;
            m_PlayBtn_MButton.interactable = !m_locked;
            m_LeftLocked_RectTransform.gameObject.SetActive(m_locked);
            m_RightLocked_RectTransform.gameObject.SetActive(m_locked);
        }
    }

    public string scene { get; set; }

    public MImage m_Star0_MImage;
    public MImage m_Star1_MImage;
    public MImage m_Star2_MImage;
    public MText m_LevelName_MText;
    public MText m_Coins_MText;
    public MText m_BestTime_MText;
    public MImage m_LevelImage_MImage;
    public MButton m_PlayBtn_MButton;
    public RectTransform m_LeftLocked_RectTransform;
    public RectTransform m_RightLocked_RectTransform;

    protected void Awake()
    {
        m_PlayBtn_MButton.onClick.AddListener(() => 
        {
            Play();
        });
    }

    public void UpdateView(GameLevel level)
    {
        if (level != null)
        {
            locked = level.locked;
            m_PlayBtn_MButton.interactable = !locked;
            m_LeftLocked_RectTransform.gameObject.SetActive(locked);
            m_RightLocked_RectTransform.gameObject.SetActive(locked);

            scene = level.scene;

            m_LevelName_MText.text = level.Name;
            m_BestTime_MText.text = GameLevel.FormattedTime(level.time);
            m_Coins_MText.text = level.coins.ToString("000");
            m_LevelImage_MImage.sprite = level.previewImage;
            m_LevelImage_MImage.color = Color.white;//将默认灰色设置为白色(显示图片原色)

            m_Star0_MImage.enabled = level.stars[0];
            m_Star1_MImage.enabled = level.stars[1];
            m_Star2_MImage.enabled = level.stars[2];
        }
    }

    public virtual void Play()
    {
        GameLoader.Instance.Load(scene, $"{ABPath.ABROOTPATH}/{scene}.unity", () =>
        {
            UIController.Instance.bottomRoot.ClosePanel(UIController.levelSelectPanelName);
        });
    }
}
