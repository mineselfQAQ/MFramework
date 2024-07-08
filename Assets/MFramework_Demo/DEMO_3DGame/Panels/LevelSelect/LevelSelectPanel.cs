using UnityEngine.UI;

public class LevelSelectPanel : LevelSelectPanelBase
{
    public void Init()
    {
        
    }

    protected override void OnClicked(Button button)
    {
        if (button == m_BackBtn_MButton)
        {
            UIController.Instance.LevelSelectBackToFileSelect();
        }
    }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
    
    protected override void OnVisibleChanged(bool visible) { }
    
    protected override void OnFocusChanged(bool focus) { }
}