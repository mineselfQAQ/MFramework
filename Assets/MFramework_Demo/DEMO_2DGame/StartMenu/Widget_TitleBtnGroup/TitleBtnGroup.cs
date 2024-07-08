using UnityEngine;
using UnityEngine.UI;

public class TitleBtnGroup : TitleBtnGroupBase
{
    public void Init()
    {
        
    }

    protected override void OnClicked(Button button)
    {
        if (button == m_StartBtn_MButton)
        {

        }
        else if (button == m_OptionsBtn_MButton)
        {
            CloseSelf();
            parentView.OpenWidget<OptionsBtnGroup>();
        }
        else if (button == m_ExitBtn_MButton)
        {
            panel.parentRoot.ClosePanel(panel.panelID, () =>
            {
                MUtility.QuitGame();
            });
        }
    }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
    
    
}