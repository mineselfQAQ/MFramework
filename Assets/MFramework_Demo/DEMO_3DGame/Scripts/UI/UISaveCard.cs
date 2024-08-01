using MFramework;
using UnityEngine;

/// <summary>
/// UI组件---保存卡，用于读取存档
/// </summary>
public class UISaveCard : ComponentSingleton<UISaveCard>
{
    [Header("List")]
    public bool focusFirstElement = true;

    [Header("Text Formatting")]
    public string retriesFormat = "00";
    public string starsFormat = "00";
    public string coinsFormat = "000";
    public string dateFormat = "MM/dd/y hh:mm";

    //[Header("Containers")]
    //public GameObject dataContainer;
    //public GameObject emptyContainer;

    //[Header("UI Elements")]
    //public MText m_retries;
    //public MText stars;
    //public MText coins;
    //public MText m_createdTime;
    //public MText m_updatedTime;
    //public MButton loadBtn;
    //public MButton deleteBtn;
    //public MButton newGameBtn;

    //protected int m_index;
    //protected GameData m_data;

    ///// <summary>
    ///// 是否已有存档
    ///// </summary>
    //public bool isFilled { get; protected set; }

    //protected virtual void Start()
    //{
    //    loadBtn.onClick.AddListener(Load);
    //    deleteBtn.onClick.AddListener(Delete);
    //    newGameBtn.onClick.AddListener(Create);
    //}

    /// <summary>
    /// 刷新数据及视图
    /// </summary>
    //public virtual void Fill(int index, GameData data)
    //{
    //    m_index = index;
    //    isFilled = data != null;
    //    dataContainer.SetActive(isFilled);
    //    emptyContainer.SetActive(!isFilled);
    //    loadBtn.interactable = isFilled;
    //    deleteBtn.interactable = isFilled;
    //    newGameBtn.interactable = !isFilled;

    //    if (data != null)
    //    {
    //        m_data = data;
    //        m_retries.text = data.m_retries.ToString(retriesFormat);
    //        stars.text = data.TotalStars().ToString(starsFormat);
    //        coins.text = data.TotalCoins().ToString(coinsFormat);
    //        m_createdTime.text = DateTime.Parse(data.m_createdTime).ToLocalTime().ToString(dateFormat);
    //        m_updatedTime.text = DateTime.Parse(data.m_updatedTime).ToLocalTime().ToString(dateFormat);
    //    }
    //}

    //public virtual void Load()
    //{
    //    Game.Instance.LoadState(m_index, m_data);
    //    UIController.Instance.FileSelectToLevelSelect();
    //}

    //public virtual void Delete()
    //{
    //    GameSaver.Instance.Delete(m_index);
    //    Fill(m_index, null);
    //    EventSystem.current.SetSelectedGameObject(newGameBtn.gameObject);
    //}

    //public virtual void Create()
    //{
    //    var data = GameData.Create();
    //    GameSaver.Instance.Save(data, m_index);
    //    Fill(m_index, data);
    //    EventSystem.current.SetSelectedGameObject(loadBtn.gameObject);
    //}
}
