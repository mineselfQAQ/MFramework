using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DEMODialogPanelBase : UIPanel
{
    protected MImage m_ProfilePhoto_MImage;
	protected MText m_Name_MText;
	protected MText m_Text_MText;
	protected MButton m_Background_MButton;

    protected override void OnBindCompsAndEvents()
    {
        m_ProfilePhoto_MImage = (MImage)viewBehaviour.GetComp(0, 0);
		m_Name_MText = (MText)viewBehaviour.GetComp(1, 0);
		m_Text_MText = (MText)viewBehaviour.GetComp(2, 0);
		m_Background_MButton = (MButton)viewBehaviour.GetComp(3, 0);
		
        BindEvent(m_Background_MButton);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_Background_MButton);
		
        m_ProfilePhoto_MImage = null;
		m_Name_MText = null;
		m_Text_MText = null;
		m_Background_MButton = null;
    }
}