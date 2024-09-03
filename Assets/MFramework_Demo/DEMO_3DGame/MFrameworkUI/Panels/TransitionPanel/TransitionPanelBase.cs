using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TransitionPanelBase : UIPanel
{
    protected UIWidgetBehaviour m_TakeBloodRestartWidget_UIWidgetBehaviour;
	protected UIWidgetBehaviour m_RestartWidget_UIWidgetBehaviour;
	protected UIWidgetBehaviour m_GameOverWidget_UIWidgetBehaviour;
	protected UIWidgetBehaviour m_LoadingWidget_UIWidgetBehaviour;

    protected override void OnBindCompsAndEvents()
    {
        m_TakeBloodRestartWidget_UIWidgetBehaviour = (UIWidgetBehaviour)viewBehaviour.GetComp(0, 0);
		m_RestartWidget_UIWidgetBehaviour = (UIWidgetBehaviour)viewBehaviour.GetComp(1, 0);
		m_GameOverWidget_UIWidgetBehaviour = (UIWidgetBehaviour)viewBehaviour.GetComp(2, 0);
		m_LoadingWidget_UIWidgetBehaviour = (UIWidgetBehaviour)viewBehaviour.GetComp(3, 0);
		
        
    }

    protected override void OnUnbindCompsAndEvents()
    {
        
        m_TakeBloodRestartWidget_UIWidgetBehaviour = null;
		m_RestartWidget_UIWidgetBehaviour = null;
		m_GameOverWidget_UIWidgetBehaviour = null;
		m_LoadingWidget_UIWidgetBehaviour = null;
    }
}