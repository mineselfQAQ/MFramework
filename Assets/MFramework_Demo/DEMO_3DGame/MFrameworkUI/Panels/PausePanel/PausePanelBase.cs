using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PausePanelBase : UIPanel
{
    protected MButton m_ResumeBtn_MButton;
	protected MButton m_CheckpointBtn_MButton;
	protected MButton m_RestartBtn_MButton;
	protected MButton m_ExitBtn_MButton;
	protected MButton m_SettingBtn_MButton;
	protected RectTransform m_PausePanel_RectTransform;

    protected override void OnBindCompsAndEvents()
    {
        m_ResumeBtn_MButton = (MButton)viewBehaviour.GetComp(0, 0);
		m_CheckpointBtn_MButton = (MButton)viewBehaviour.GetComp(1, 0);
		m_RestartBtn_MButton = (MButton)viewBehaviour.GetComp(2, 0);
		m_ExitBtn_MButton = (MButton)viewBehaviour.GetComp(3, 0);
		m_SettingBtn_MButton = (MButton)viewBehaviour.GetComp(4, 0);
		m_PausePanel_RectTransform = (RectTransform)viewBehaviour.GetComp(5, 0);
		
        BindEvent(m_ResumeBtn_MButton);
		BindEvent(m_CheckpointBtn_MButton);
		BindEvent(m_RestartBtn_MButton);
		BindEvent(m_ExitBtn_MButton);
		BindEvent(m_SettingBtn_MButton);
    }

    protected override void OnUnbindCompsAndEvents()
    {
        UnbindEvent(m_ResumeBtn_MButton);
		UnbindEvent(m_CheckpointBtn_MButton);
		UnbindEvent(m_RestartBtn_MButton);
		UnbindEvent(m_ExitBtn_MButton);
		UnbindEvent(m_SettingBtn_MButton);
		
        m_ResumeBtn_MButton = null;
		m_CheckpointBtn_MButton = null;
		m_RestartBtn_MButton = null;
		m_ExitBtn_MButton = null;
		m_SettingBtn_MButton = null;
		m_PausePanel_RectTransform = null;
    }
}