using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectPanelBase : UIPanel
{
    protected MButton m_BackBtn_MButton;

    protected override void OnBindCompsAndEvents()
    {
        m_BackBtn_MButton = (MButton)viewBehaviour.GetComp(0, 0);
		
        BindEvent(m_BackBtn_MButton);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_BackBtn_MButton);
		
        m_BackBtn_MButton = null;
    }
}