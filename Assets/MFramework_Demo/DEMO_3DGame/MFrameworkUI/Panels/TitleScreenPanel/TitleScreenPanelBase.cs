using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleScreenPanelBase : UIPanel
{
    protected MButton m_StartBtn_MButton;

    protected override void OnBindCompsAndEvents()
    {
        m_StartBtn_MButton = (MButton)viewBehaviour.GetComp(0, 0);
		
        BindEvent(m_StartBtn_MButton);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_StartBtn_MButton);
		
        m_StartBtn_MButton = null;
    }
}