using MFramework.UI;
using UnityEngine;

public class UILevelCard : MonoBehaviour
{
    public MImage[] starsImages;
    public MText title;
    public MText coins;
    public MText time;
    public MImage image;
    public MButton playBtn;

    protected bool m_locked;

    public string scene { get; set; }

    public bool locked
    {
        get { return m_locked; }

        set
        {
            m_locked = value;
            playBtn.interactable = !m_locked;
        }
    }

    protected virtual void Start()
    {
        playBtn.onClick.AddListener(Play);
    }

    public virtual void Play()
    {
        GameLoader.Instance.Load(scene, () => 
        {
            UIController.Instance.bottomRoot.ClosePanel("LevelSelect");
        });
    }

    public virtual void Fill(GameLevel level)
    {
        if (level != null)
        {
            locked = level.locked;
            scene = level.scene;
            title.text = level.name;
            time.text = GameLevel.FormattedTime(level.time);
            coins.text = level.coins.ToString("000");
            image.sprite = level.image;

            for (int i = 0; i < starsImages.Length; i++)
            {
                starsImages[i].enabled = level.stars[i];
            }
        }
    }

}