using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelCardWidgetBase : UIWidget
{
    protected MImage m_Star0_MImage;
	protected MImage m_Star1_MImage;
	protected MImage m_Star2_MImage;
	protected MText m_LevelName_MText;
	protected MText m_Coins_MText;
	protected MText m_BestTime_MText;
	protected MImage m_LevelImage_MImage;
	internal MButton m_PlayBtn_MButton;

    protected override void OnBindCompsAndEvents()
    {
        m_Star0_MImage = (MImage)viewBehaviour.GetComp(0, 0);
		m_Star1_MImage = (MImage)viewBehaviour.GetComp(1, 0);
		m_Star2_MImage = (MImage)viewBehaviour.GetComp(2, 0);
		m_LevelName_MText = (MText)viewBehaviour.GetComp(3, 0);
		m_Coins_MText = (MText)viewBehaviour.GetComp(4, 0);
		m_BestTime_MText = (MText)viewBehaviour.GetComp(5, 0);
		m_LevelImage_MImage = (MImage)viewBehaviour.GetComp(6, 0);
		m_PlayBtn_MButton = (MButton)viewBehaviour.GetComp(7, 0);
		
        BindEvent(m_PlayBtn_MButton);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_PlayBtn_MButton);
		
        m_Star0_MImage = null;
		m_Star1_MImage = null;
		m_Star2_MImage = null;
		m_LevelName_MText = null;
		m_Coins_MText = null;
		m_BestTime_MText = null;
		m_LevelImage_MImage = null;
		m_PlayBtn_MButton = null;
    }
}