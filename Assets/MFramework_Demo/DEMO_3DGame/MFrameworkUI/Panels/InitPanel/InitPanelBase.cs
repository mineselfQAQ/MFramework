using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InitPanelBase : UIPanel
{
    protected Slider m_MSlider_Slider;
	protected MText m_MText_MText;

    protected override void OnBindCompsAndEvents()
    {
        m_MSlider_Slider = (Slider)viewBehaviour.GetComp(0, 0);
		m_MText_MText = (MText)viewBehaviour.GetComp(1, 0);
		
        BindEvent(m_MSlider_Slider);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_MSlider_Slider);
		
        m_MSlider_Slider = null;
		m_MText_MText = null;
    }
}