using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectPanelBase : UIPanel
{
    protected MButton m_BackBtn_MButton;
	protected RectTransform m_Content_RectTransform;
	protected LevelScrollView m_LevelScrollView_LevelScrollView;

    protected override void OnBindCompsAndEvents()
    {
        m_BackBtn_MButton = (MButton)viewBehaviour.GetComp(0, 0);
		m_Content_RectTransform = (RectTransform)viewBehaviour.GetComp(1, 0);
		m_LevelScrollView_LevelScrollView = (LevelScrollView)viewBehaviour.GetComp(2, 0);
		
        BindEvent(m_BackBtn_MButton);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_BackBtn_MButton);
		
        m_BackBtn_MButton = null;
		m_Content_RectTransform = null;
		m_LevelScrollView_LevelScrollView = null;
    }
}