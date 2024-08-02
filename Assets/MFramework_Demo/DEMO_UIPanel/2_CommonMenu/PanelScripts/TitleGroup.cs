using MFramework;
using UnityEngine.UI;

public class TitleGroup : TitleGroupBase
{
    public override void Init()
    {
        
    }

    protected override void OnClicked(Button button) 
    {
        if (button == m_StartBtn_Button)
        {
            MLog.Print("已进入游戏");
        }
        else if (button == m_SettingBtn_Button)
        {
            CloseSelf();
            parentView.OpenWidget<SettingGroup>();
        }
        else if (button == m_ExitBtn_Button)
        {
            panel.parentRoot.ClosePanel(panel.panelID, () => 
            {
                MLog.Print("已退出游戏");
                MUtility.QuitGame();
            });
            
        }
    }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
    
    
}