﻿using System.Collections.Generic;
using UnityEngine.EventSystems;

public class FileSelectPanel : FileSelectPanelBase
{
    protected List<SaveCardWidget> m_cardList = new List<SaveCardWidget>();//存档列表

    public void Init()
    {
        //获取JSON中存储的GameData数据
        var data = GameSaver.Instance.LoadList();

        //创建视图
        for (int i = 0; i < data.Length; i++)
        {
            SaveCardWidget saveCard = CreateWidget<SaveCardWidget>($"SaveCard_{i}", m_Content_RectTransform,
                $"{UIController.widgetPrepath}/SaveCard/SaveCardWidget.prefab", true);
            saveCard.Init(i, data[i]);
            m_cardList.Add(saveCard);
        }

        if (UISaveCard.Instance.focusFirstElement)
        {
            if (m_cardList[0].isFilled)
            {
                EventSystem.current.SetSelectedGameObject(m_cardList[0].m_LoadBtn_MButton.gameObject);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(m_cardList[0].m_NewGameBtn_MButton.gameObject);
            }
        }
    }
}