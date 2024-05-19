using MFramework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FuncTest_PanelBase : UIPanel
{
    protected RectTransform m_Widget1Group_RectTransform;
	protected UIWidgetBehaviour m_FuncTest_Widget2_UIWidgetBehaviour;

    protected override void OnBindCompsAndEvents()
    {
        m_Widget1Group_RectTransform = (RectTransform)viewBehaviour.GetComp(0, 0);
		m_FuncTest_Widget2_UIWidgetBehaviour = (UIWidgetBehaviour)viewBehaviour.GetComp(1, 0);
		
        
    }

    protected override void OnUnbindCompsAndEvents()
    {
        
        m_Widget1Group_RectTransform = null;
		m_FuncTest_Widget2_UIWidgetBehaviour = null;
    }
}