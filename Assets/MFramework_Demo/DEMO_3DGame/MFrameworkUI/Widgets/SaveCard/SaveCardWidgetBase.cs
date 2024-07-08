using MFramework;
using MFramework.UI;
using UnityEngine;

public class SaveCardWidgetBase : UIWidget
{
    protected RectTransform m_Data_RectTransform;
	protected RectTransform m_Empty_RectTransform;
	protected MText m_Retries_MText;
	protected MText m_Star_MText;
	protected MText m_Coin_MText;
	protected MText m_CreatedTime_MText;
	protected MText m_UpdatedTime_MText;
	internal MButton m_LoadBtn_MButton;
    internal MButton m_DeleteBtn_MButton;
    internal MButton m_NewGameBtn_MButton;

    protected override void OnBindCompsAndEvents()
    {
        m_Data_RectTransform = (RectTransform)viewBehaviour.GetComp(0, 0);
		m_Empty_RectTransform = (RectTransform)viewBehaviour.GetComp(1, 0);
		m_Retries_MText = (MText)viewBehaviour.GetComp(2, 0);
		m_Star_MText = (MText)viewBehaviour.GetComp(3, 0);
		m_Coin_MText = (MText)viewBehaviour.GetComp(4, 0);
		m_CreatedTime_MText = (MText)viewBehaviour.GetComp(5, 0);
		m_UpdatedTime_MText = (MText)viewBehaviour.GetComp(6, 0);
		m_LoadBtn_MButton = (MButton)viewBehaviour.GetComp(7, 0);
		m_DeleteBtn_MButton = (MButton)viewBehaviour.GetComp(8, 0);
		m_NewGameBtn_MButton = (MButton)viewBehaviour.GetComp(9, 0);
		
        BindEvent(m_LoadBtn_MButton);
		BindEvent(m_DeleteBtn_MButton);
		BindEvent(m_NewGameBtn_MButton);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_LoadBtn_MButton);
		UnbindEvent(m_DeleteBtn_MButton);
		UnbindEvent(m_NewGameBtn_MButton);
		
        m_Data_RectTransform = null;
		m_Empty_RectTransform = null;
		m_Retries_MText = null;
		m_Star_MText = null;
		m_Coin_MText = null;
		m_CreatedTime_MText = null;
		m_UpdatedTime_MText = null;
		m_LoadBtn_MButton = null;
		m_DeleteBtn_MButton = null;
		m_NewGameBtn_MButton = null;
    }
}