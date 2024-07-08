using MFramework;
using UnityEngine.UI;
using TMPro;

public class TitleMenuBase : UIPanel
{
    protected Image m_Background_Image;
	protected TextMeshProUGUI m_Title_TMPText;
	protected UIWidgetBehaviour m_TitleGroup_UIWidgetBehaviour;
	protected UIWidgetBehaviour m_SettingGroup_UIWidgetBehaviour;

    protected override void OnBindCompsAndEvents()
    {
        m_Background_Image = (Image)viewBehaviour.GetComp(0, 0);
		m_Title_TMPText = (TextMeshProUGUI)viewBehaviour.GetComp(1, 0);
		m_TitleGroup_UIWidgetBehaviour = (UIWidgetBehaviour)viewBehaviour.GetComp(2, 0);
		m_SettingGroup_UIWidgetBehaviour = (UIWidgetBehaviour)viewBehaviour.GetComp(3, 0);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        m_Background_Image = null;
		m_Title_TMPText = null;
		m_TitleGroup_UIWidgetBehaviour = null;
		m_SettingGroup_UIWidgetBehaviour = null;
    }
}