﻿using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectPanel : LevelSelectPanelBase
{
    protected List<LevelCardWidget> m_cardList = new List<LevelCardWidget>();

    public void Init()
    {
        var levels = Game.Instance.levels;

        for (int i = 0; i < levels.Count; i++)
        {
            LevelCardWidget levelCard = CreateWidget<LevelCardWidget>($"LevelCard_{i}", m_Content_RectTransform,
                $"{UIController.widgetPrepath}/LevelCard/LevelCardWidget.prefab", true);
            levelCard.Init(levels[i]);
            m_cardList.Add(levelCard);
        }

        if (true)
        {
            EventSystem.current.SetSelectedGameObject(m_cardList[0].m_PlayBtn_MButton.gameObject);
        }
    }

    protected override void OnClicked(Button button)
    {
        if (button == m_BackBtn_MButton)
        {
            UIController.Instance.LevelSelectBackToFileSelect();
        }
    }
}