using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamepadBase : UIPanel
{
    protected RectTransform m_Gamepad_RectTransform;

    protected override void OnBindCompsAndEvents()
    {
        m_Gamepad_RectTransform = (RectTransform)viewBehaviour.GetComp(0, 0);
		
        
    }

    protected override void OnUnbindCompsAndEvents()
    {
        
        m_Gamepad_RectTransform = null;
    }
}