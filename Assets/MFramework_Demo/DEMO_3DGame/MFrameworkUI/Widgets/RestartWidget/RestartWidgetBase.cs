using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestartWidgetBase : UIWidget
{
    protected MText m_RetriesNum_MText;

    protected override void OnBindCompsAndEvents()
    {
        m_RetriesNum_MText = (MText)viewBehaviour.GetComp(0, 0);
		
        
    }

    protected override void OnUnbindCompsAndEvents()
    {
        
        m_RetriesNum_MText = null;
    }
}