using MFramework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MPanelBase : UIPanel
{
    protected MButton m_Button_MButton;

    protected override void OnBindCompsAndEvents()
    {
        m_Button_MButton = (MButton)viewBehaviour.GetComp(0, 0);
        BindEvent(m_Button_MButton);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_Button_MButton);
        m_Button_MButton = null;
    }
}