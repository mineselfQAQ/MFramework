using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleBtnGroupBase : UIWidget
{
    protected MButton m_StartBtn_MButton;
	protected MButton m_OptionsBtn_MButton;
	protected MButton m_ExitBtn_MButton;

    protected override void OnBindCompsAndEvents()
    {
        m_StartBtn_MButton = (MButton)viewBehaviour.GetComp(0, 0);
		m_OptionsBtn_MButton = (MButton)viewBehaviour.GetComp(1, 0);
		m_ExitBtn_MButton = (MButton)viewBehaviour.GetComp(2, 0);
		
        BindEvent(m_StartBtn_MButton);
		BindEvent(m_OptionsBtn_MButton);
		BindEvent(m_ExitBtn_MButton);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_StartBtn_MButton);
		UnbindEvent(m_OptionsBtn_MButton);
		UnbindEvent(m_ExitBtn_MButton);
		
        m_StartBtn_MButton = null;
		m_OptionsBtn_MButton = null;
		m_ExitBtn_MButton = null;
    }
}