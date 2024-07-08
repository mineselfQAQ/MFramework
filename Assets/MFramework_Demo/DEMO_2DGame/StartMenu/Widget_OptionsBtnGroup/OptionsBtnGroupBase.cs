using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsBtnGroupBase : UIWidget
{
    protected MButton m_SoundBtn_MButton;
	protected MButton m_LanguageBtn_MButton;
	protected MText m_SoundText_MText;
	protected MText m_LanguageText_MText;

    protected override void OnBindCompsAndEvents()
    {
        m_SoundBtn_MButton = (MButton)viewBehaviour.GetComp(0, 0);
		m_LanguageBtn_MButton = (MButton)viewBehaviour.GetComp(1, 0);
		m_SoundText_MText = (MText)viewBehaviour.GetComp(2, 0);
		m_LanguageText_MText = (MText)viewBehaviour.GetComp(3, 0);
		
        BindEvent(m_SoundBtn_MButton);
		BindEvent(m_LanguageBtn_MButton);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_SoundBtn_MButton);
		UnbindEvent(m_LanguageBtn_MButton);
		
        m_SoundBtn_MButton = null;
		m_LanguageBtn_MButton = null;
		m_SoundText_MText = null;
		m_LanguageText_MText = null;
    }
}