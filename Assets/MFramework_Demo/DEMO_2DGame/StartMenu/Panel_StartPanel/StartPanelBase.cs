using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartPanelBase : UIPanel
{
    protected MImage m_MBackground_MImage;
	protected MText m_Title_MText;
	protected UIWidgetBehaviour m_TitleBtnGroup_UIWidgetBehaviour;
	protected UIWidgetBehaviour m_OptionsBtnGroup_UIWidgetBehaviour;
	protected MButton m_PointChecker_MButton;

    protected override void OnBindCompsAndEvents()
    {
        m_MBackground_MImage = (MImage)viewBehaviour.GetComp(0, 0);
		m_Title_MText = (MText)viewBehaviour.GetComp(1, 0);
		m_TitleBtnGroup_UIWidgetBehaviour = (UIWidgetBehaviour)viewBehaviour.GetComp(2, 0);
		m_OptionsBtnGroup_UIWidgetBehaviour = (UIWidgetBehaviour)viewBehaviour.GetComp(3, 0);
		m_PointChecker_MButton = (MButton)viewBehaviour.GetComp(4, 0);
		
        BindEvent(m_PointChecker_MButton);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_PointChecker_MButton);
		
        m_MBackground_MImage = null;
		m_Title_MText = null;
		m_TitleBtnGroup_UIWidgetBehaviour = null;
		m_OptionsBtnGroup_UIWidgetBehaviour = null;
		m_PointChecker_MButton = null;
    }
}