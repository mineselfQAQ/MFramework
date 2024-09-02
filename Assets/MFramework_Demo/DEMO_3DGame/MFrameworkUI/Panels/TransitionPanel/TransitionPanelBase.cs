using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TransitionPanelBase : UIPanel
{
    protected UIWidgetBehaviour m_TakeBloodRestartWidget_UIWidgetBehaviour;

    protected override void OnBindCompsAndEvents()
    {
        m_TakeBloodRestartWidget_UIWidgetBehaviour = (UIWidgetBehaviour)viewBehaviour.GetComp(0, 0);
		
        
    }

    protected override void OnUnbindCompsAndEvents()
    {
        
        m_TakeBloodRestartWidget_UIWidgetBehaviour = null;
    }
}