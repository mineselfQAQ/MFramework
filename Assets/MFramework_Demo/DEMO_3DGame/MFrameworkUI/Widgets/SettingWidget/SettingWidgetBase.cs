using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingWidgetBase : UIWidget
{
    protected MButton m_CloseBtn_MButton;
	protected MButton m_LanguageBtn_MButton;
	protected Slider m_SFXSlider_Slider;
	protected Slider m_MusicSlider_Slider;
	protected MText m_LanguageText_MText;
	protected MImage m_LanguageIcon_MImage;
	protected LanguageInfos m_SettingWidget_LanguageInfos;

    protected override void OnBindCompsAndEvents()
    {
        m_CloseBtn_MButton = (MButton)viewBehaviour.GetComp(0, 0);
		m_LanguageBtn_MButton = (MButton)viewBehaviour.GetComp(1, 0);
		m_SFXSlider_Slider = (Slider)viewBehaviour.GetComp(2, 0);
		m_MusicSlider_Slider = (Slider)viewBehaviour.GetComp(3, 0);
		m_LanguageText_MText = (MText)viewBehaviour.GetComp(4, 0);
		m_LanguageIcon_MImage = (MImage)viewBehaviour.GetComp(5, 0);
		m_SettingWidget_LanguageInfos = (LanguageInfos)viewBehaviour.GetComp(6, 0);
		
        BindEvent(m_CloseBtn_MButton);
		BindEvent(m_LanguageBtn_MButton);
		BindEvent(m_SFXSlider_Slider);
		BindEvent(m_MusicSlider_Slider);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_CloseBtn_MButton);
		UnbindEvent(m_LanguageBtn_MButton);
		UnbindEvent(m_SFXSlider_Slider);
		UnbindEvent(m_MusicSlider_Slider);
		
        m_CloseBtn_MButton = null;
		m_LanguageBtn_MButton = null;
		m_SFXSlider_Slider = null;
		m_MusicSlider_Slider = null;
		m_LanguageText_MText = null;
		m_LanguageIcon_MImage = null;
		m_SettingWidget_LanguageInfos = null;
    }
}