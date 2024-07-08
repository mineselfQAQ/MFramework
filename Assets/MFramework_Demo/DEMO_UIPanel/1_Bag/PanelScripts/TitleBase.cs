using MFramework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleBase : UIWidget
{
    protected TextMeshProUGUI m_Name_TMPText;
	protected TextMeshProUGUI m_ID_TMPText;

    protected override void OnBindCompsAndEvents()
    {
        m_Name_TMPText = (TextMeshProUGUI)viewBehaviour.GetComp(0, 0);
		m_ID_TMPText = (TextMeshProUGUI)viewBehaviour.GetComp(1, 0);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        m_Name_TMPText = null;
		m_ID_TMPText = null;
    }
}