using UnityEngine.UI;

public class TitleScreenPanel : TitleScreenPanelBase
{
    public override void Init()
    {
        
    }

    protected override void OnClicked(Button button) 
    {
        if (button == m_StartBtn_MButton)
        {
            UIController.Instance.TitleScreenToFileSelect();
        }
    }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
    
    protected override void OnVisibleChanged(bool visible) { }
    
    protected override void OnFocusChanged(bool focus) { }
}