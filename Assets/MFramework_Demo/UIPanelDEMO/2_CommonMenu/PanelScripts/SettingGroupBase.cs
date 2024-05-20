using MFramework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingGroupBase : UIWidget
{
    protected Button m_LanguageBtn_Button;
	protected Button m_BackBtn_Button;
	protected TextMeshProUGUI m_LanguageText_Text;

    protected override void OnBindCompsAndEvents()
    {
        m_LanguageBtn_Button = (Button)viewBehaviour.GetComp(0, 0);
		m_BackBtn_Button = (Button)viewBehaviour.GetComp(1, 0);
        m_LanguageText_Text = (TextMeshProUGUI)viewBehaviour.GetComp(2, 0);
		
        BindEvent(m_LanguageBtn_Button);
		BindEvent(m_BackBtn_Button);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_LanguageBtn_Button);
		UnbindEvent(m_BackBtn_Button);
		
        m_LanguageBtn_Button = null;
		m_BackBtn_Button = null;
        m_LanguageText_Text = null;
    }
}