using MFramework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestPanelBase : UIPanel
{
    protected Button m_Button1_Button;
	protected Button m_Button2_Button;
	protected Image m_Button2_Image;
	protected CanvasRenderer m_Button2_CanvasRenderer;
	protected RectTransform m_Button2_RectTransform;
	protected Animator m_TestPanel_Animator;

    protected override void OnBindCompsAndEvents()
    {
        m_Button1_Button = (Button)viewBehaviour.GetComp(0, 0);
		m_Button2_Button = (Button)viewBehaviour.GetComp(1, 0);
		m_Button2_Image = (Image)viewBehaviour.GetComp(1, 1);
		m_Button2_CanvasRenderer = (CanvasRenderer)viewBehaviour.GetComp(1, 2);
		m_Button2_RectTransform = (RectTransform)viewBehaviour.GetComp(1, 3);
		m_TestPanel_Animator = (Animator)viewBehaviour.GetComp(2, 0);
		
        BindEvent(m_Button1_Button);
		BindEvent(m_Button2_Button);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_Button1_Button);
		UnbindEvent(m_Button2_Button);
		
        m_Button1_Button = null;
		m_Button2_Button = null;
		m_Button2_Image = null;
		m_Button2_CanvasRenderer = null;
		m_Button2_RectTransform = null;
		m_TestPanel_Animator = null;
    }
}