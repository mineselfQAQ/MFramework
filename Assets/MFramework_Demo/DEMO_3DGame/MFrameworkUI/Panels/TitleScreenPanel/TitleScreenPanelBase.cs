using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleScreenPanelBase : UIPanel
{
    protected MButton m_StartBtn_MButton;
	protected MButton m_CNBtn_MButton;
	protected MButton m_ENBtn_MButton;

    protected override void OnBindCompsAndEvents()
    {
        m_StartBtn_MButton = (MButton)viewBehaviour.GetComp(0, 0);
		m_CNBtn_MButton = (MButton)viewBehaviour.GetComp(1, 0);
		m_ENBtn_MButton = (MButton)viewBehaviour.GetComp(2, 0);
		
        BindEvent(m_StartBtn_MButton);
		BindEvent(m_CNBtn_MButton);
		BindEvent(m_ENBtn_MButton);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_StartBtn_MButton);
		UnbindEvent(m_CNBtn_MButton);
		UnbindEvent(m_ENBtn_MButton);
		
        m_StartBtn_MButton = null;
		m_CNBtn_MButton = null;
		m_ENBtn_MButton = null;
    }
}