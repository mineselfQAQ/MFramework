using MFramework;
using MFramework.UI;

public class HUDPanelBase : UIPanel
{
    protected MText m_HeartNum_MText;
	protected MText m_RetriesNum_MText;
	protected MText m_CoinNum_MText;
	protected MText m_Timer_MText;
	protected MImage m_Star0_MImage;
	protected MImage m_Star1_MImage;
	protected MImage m_Star2_MImage;

    protected override void OnBindCompsAndEvents()
    {
        m_HeartNum_MText = (MText)viewBehaviour.GetComp(0, 0);
		m_RetriesNum_MText = (MText)viewBehaviour.GetComp(1, 0);
		m_CoinNum_MText = (MText)viewBehaviour.GetComp(2, 0);
		m_Timer_MText = (MText)viewBehaviour.GetComp(3, 0);
		m_Star0_MImage = (MImage)viewBehaviour.GetComp(4, 0);
		m_Star1_MImage = (MImage)viewBehaviour.GetComp(5, 0);
		m_Star2_MImage = (MImage)viewBehaviour.GetComp(6, 0);
		
        
    }

    protected override void OnUnbindCompsAndEvents()
    {
        
        m_HeartNum_MText = null;
		m_RetriesNum_MText = null;
		m_CoinNum_MText = null;
		m_Timer_MText = null;
		m_Star0_MImage = null;
		m_Star1_MImage = null;
		m_Star2_MImage = null;
    }
}