using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FileSelectPanelBase : UIPanel
{
    protected RectTransform m_Content_RectTransform;

    protected override void OnBindCompsAndEvents()
    {
        m_Content_RectTransform = (RectTransform)viewBehaviour.GetComp(0, 0);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        
        m_Content_RectTransform = null;
    }
}