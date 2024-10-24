using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPanel : LevelSelectPanelBase
{
    protected List<LevelCell> m_cardList = new List<LevelCell>();

    public override void Init()
    {
        var levels = Game.Instance.levels;
        m_LevelScrollView_LevelScrollView.InitView(levels);

        //强制移动至头
        if (UILevelCard.Instance.focusFirstElement)
        {
            m_LevelScrollView_LevelScrollView.MoveTo(0);
        }
    }

    public void Refresh()
    {
        var levels = Game.Instance.levels;
        m_LevelScrollView_LevelScrollView.RefreshView();

        //强制移动至头
        if (UILevelCard.Instance.focusFirstElement)
        {
            m_LevelScrollView_LevelScrollView.MoveTo(0);
        }
    }

    protected override void OnClicked(Button button)
    {
        if (button == m_BackBtn_MButton)
        {
            UIController.Instance.LevelSelectBackToFileSelect();
        }
    }

    protected override GameObject LoadPrefab(string prefabPath)
    {
        return ABUtitlity.LoadPanelSync(prefabPath);
    }
}