using MFramework.UI;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI���---���濨�����ڶ�ȡ�浵
/// </summary>
public class UISaveCard : MonoBehaviour
{
    [Header("Text Formatting")]
    public string retriesFormat = "00";
    public string starsFormat = "00";
    public string coinsFormat = "000";
    public string dateFormat = "MM/dd/y hh:mm";

    [Header("Containers")]
    public GameObject dataContainer;
    public GameObject emptyContainer;

    [Header("UI Elements")]
    public MText retries;
    public MText stars;
    public MText coins;
    public MText createdTime;
    public MText updatedTime;
    public MButton loadBtn;
    public MButton deleteBtn;
    public MButton newGameBtn;

    protected int m_index;
    protected GameData m_data;

    /// <summary>
    /// �Ƿ����д浵
    /// </summary>
    public bool isFilled { get; protected set; }

    protected virtual void Start()
    {
        loadBtn.onClick.AddListener(Load);
        deleteBtn.onClick.AddListener(Delete);
        newGameBtn.onClick.AddListener(Create);
    }

    /// <summary>
    /// ˢ�����ݼ���ͼ
    /// </summary>
    public virtual void Fill(int index, GameData data)
    {
        m_index = index;
        isFilled = data != null;
        dataContainer.SetActive(isFilled);
        emptyContainer.SetActive(!isFilled);
        loadBtn.interactable = isFilled;
        deleteBtn.interactable = isFilled;
        newGameBtn.interactable = !isFilled;

        if (data != null)
        {
            m_data = data;
            retries.text = data.retries.ToString(retriesFormat);
            stars.text = data.TotalStars().ToString(starsFormat);
            coins.text = data.TotalCoins().ToString(coinsFormat);
            createdTime.text = DateTime.Parse(data.createdTime).ToLocalTime().ToString(dateFormat);
            updatedTime.text = DateTime.Parse(data.updatedTime).ToLocalTime().ToString(dateFormat);
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
        EventSystem.current.SetSelectedGameObject(newGameBtn.gameObject);
    }

    public virtual void Create()
    {
        var data = GameData.Create();
        GameSaver.Instance.Save(data, m_index);
        Fill(m_index, data);
        EventSystem.current.SetSelectedGameObject(loadBtn.gameObject);
    }
}