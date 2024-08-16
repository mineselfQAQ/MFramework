using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveCardWidget : SaveCardWidgetBase
{
    protected int m_index;
    protected GameData m_data;

    /// <summary>
    /// 是否已有存档
    /// </summary>
    public bool isFilled { get; protected set; }

    public void Init(int index, GameData data)
    {
        Fill(index, data);
    }
    public void Refresh(int index, GameData data)
    {
        Fill(index, data);
    }

    protected override void OnClicked(Button button)
    {
        if (button == m_NewGameBtn_MButton)
        {
            Create();
        }
        else if (button == m_LoadBtn_MButton)
        {
            Load();
        }
        else if (button == m_DeleteBtn_MButton)
        {
            Delete();
        }
    }

    /// <summary>
    /// 刷新数据及视图
    /// </summary>
    public virtual void Fill(int index, GameData data)
    {
        m_index = index;
        isFilled = data != null;
        m_Data_RectTransform.gameObject.SetActive(isFilled);
        m_Empty_RectTransform.gameObject.SetActive(!isFilled);
        m_LoadBtn_MButton.interactable = isFilled;
        m_DeleteBtn_MButton.interactable = isFilled;
        m_NewGameBtn_MButton.interactable = !isFilled;

        if (data != null)
        {
            m_data = data;
            m_Retries_MText.text = data.retries.ToString(UISaveCard.Instance.retriesFormat);
            m_Star_MText.text = data.TotalStars().ToString(UISaveCard.Instance.starsFormat);
            m_Coin_MText.text = data.TotalCoins().ToString(UISaveCard.Instance.coinsFormat);
            m_CreatedTime_MText.text = DateTime.Parse(data.createdTime).ToLocalTime().ToString(UISaveCard.Instance.dateFormat);
            m_UpdatedTime_MText.text = DateTime.Parse(data.updatedTime).ToLocalTime().ToString(UISaveCard.Instance.dateFormat);
        }
    }

    public virtual void Load()
    {
        Game.Instance.LoadState(m_index, m_data);
        UIController.Instance.FileSelectToLevelSelect();
    }

    public virtual void Delete()
    {
        GameSaver.Instance.Delete(m_index);
        Fill(m_index, null);
        EventSystem.current.SetSelectedGameObject(m_NewGameBtn_MButton.gameObject);
    }

    public virtual void Create()
    {
        var data = GameData.Create();
        GameSaver.Instance.Save(data, m_index);
        Fill(m_index, data);
        EventSystem.current.SetSelectedGameObject(m_LoadBtn_MButton.gameObject);
    }
}