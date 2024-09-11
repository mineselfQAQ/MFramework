using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogPanelBase : UIPanel
{
    protected MImage m_ProfilePhoto_MImage;
	protected MText m_Name_MText;
	protected MText m_Message_MText;
	protected MButton m_SpeechBubble_MButton;
	protected Bouncer m_Icon_Arrow_Bouncer;

    protected override void OnBindCompsAndEvents()
    {
        m_ProfilePhoto_MImage = (MImage)viewBehaviour.GetComp(0, 0);
		m_Name_MText = (MText)viewBehaviour.GetComp(1, 0);
		m_Message_MText = (MText)viewBehaviour.GetComp(2, 0);
		m_SpeechBubble_MButton = (MButton)viewBehaviour.GetComp(3, 0);
		m_Icon_Arrow_Bouncer = (Bouncer)viewBehaviour.GetComp(4, 0);
		
        BindEvent(m_SpeechBubble_MButton);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_SpeechBubble_MButton);
		
        m_ProfilePhoto_MImage = null;
		m_Name_MText = null;
		m_Message_MText = null;
		m_SpeechBubble_MButton = null;
		m_Icon_Arrow_Bouncer = null;
    }
}