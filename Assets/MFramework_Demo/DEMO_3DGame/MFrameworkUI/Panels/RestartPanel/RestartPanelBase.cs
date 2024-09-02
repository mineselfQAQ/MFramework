using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestartPanelBase : UIPanel
{
    protected MText m_OldRetriesNum_MText;
	protected MText m_NewRetriesNum_MText;

    protected override void OnBindCompsAndEvents()
    {
        m_OldRetriesNum_MText = (MText)viewBehaviour.GetComp(0, 0);
		m_NewRetriesNum_MText = (MText)viewBehaviour.GetComp(1, 0);
		
        
    }

    protected override void OnUnbindCompsAndEvents()
    {
        
        m_OldRetriesNum_MText = null;
		m_NewRetriesNum_MText = null;
    }
}