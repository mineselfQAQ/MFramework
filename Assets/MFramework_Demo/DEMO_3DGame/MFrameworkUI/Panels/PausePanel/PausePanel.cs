using UnityEngine.UI;

public class PausePanel : PausePanelBase
{
    protected SettingWidget settingWidget;

    public static readonly string settingWdigetName = "SETTING";

    public override void Init()
    {
        settingWidget = CreateWidget<SettingWidget>(settingWdigetName, m_PausePanel_RectTransform,
                $"{UIController.widgetPrepath}/SettingWidget/SettingWidget.prefab", false);
        settingWidget.Init();
    }

    protected override void OnClicked(Button button) 
    {
        if (button == m_ResumeBtn_MButton)//取消暂停
        {
            LevelPauser.Instance.Pause(false);
        }
        else if (button == m_CheckpointBtn_MButton)//重开
        {
            LevelRespawner.Instance.Respawn(false);
        }
        else if (button == m_RestartBtn_MButton)//重开
        {
            LevelRespawner.Instance.Restart();
        }
        else if (button == m_ExitBtn_MButton)//退出
        {
            LevelFinisher.Instance.Exit();
        }
        else if (button == m_SettingBtn_MButton)//设置
        {
            OpenWidget("SETTING");
        }
    }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
    
    protected override void OnVisibleChanged(bool visible) { }
    
    protected override void OnFocusChanged(bool focus) { }
}