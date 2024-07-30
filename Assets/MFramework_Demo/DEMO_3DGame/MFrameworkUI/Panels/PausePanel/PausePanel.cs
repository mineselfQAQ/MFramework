using UnityEngine.UI;

public class PausePanel : PausePanelBase
{
    public void Init()
    {
        
    }

    protected override void OnClicked(Button button) 
    {
        if (button == m_ResumeBtn_MButton)
        {

        }
        else if (button == m_CheckpointBtn_MButton)
        {

        }
        else if (button == m_RestartBtn_MButton)
        {

        }
        else if (button == m_ExitBtn_MButton)
        {

        }
    }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
    
    protected override void OnVisibleChanged(bool visible) { }
    
    protected override void OnFocusChanged(bool focus) { }
}