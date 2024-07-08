using MFramework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleGroupBase : UIWidget
{
    protected Button m_StartBtn_Button;
	protected Button m_SettingBtn_Button;
	protected Button m_ExitBtn_Button;

    protected override void OnBindCompsAndEvents()
    {
        m_StartBtn_Button = (Button)viewBehaviour.GetComp(0, 0);
		m_SettingBtn_Button = (Button)viewBehaviour.GetComp(1, 0);
		m_ExitBtn_Button = (Button)viewBehaviour.GetComp(2, 0);
		
        BindEvent(m_StartBtn_Button);
		BindEvent(m_SettingBtn_Button);
		BindEvent(m_ExitBtn_Button);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_StartBtn_Button);
		UnbindEvent(m_SettingBtn_Button);
		UnbindEvent(m_ExitBtn_Button);
		
        m_StartBtn_Button = null;
		m_SettingBtn_Button = null;
		m_ExitBtn_Button = null;
    }
}