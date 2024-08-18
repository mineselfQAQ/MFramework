using MFramework;
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
        else if (button == m_CNBtn_MButton)
        {
            if (MLocalizationManager.Instance.CurrentLanguage != SupportLanguage.CHINESE)
            {
                MLocalizationManager.Instance.SetLanguage(SupportLanguage.CHINESE);
            }
        }
        else if (button == m_ENBtn_MButton)
        {
            if (MLocalizationManager.Instance.CurrentLanguage != SupportLanguage.ENGLISH)
            {
                MLocalizationManager.Instance.SetLanguage(SupportLanguage.ENGLISH);
            }
        }
    }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
    
    protected override void OnVisibleChanged(bool visible) { }
    
    protected override void OnFocusChanged(bool focus) { }
}