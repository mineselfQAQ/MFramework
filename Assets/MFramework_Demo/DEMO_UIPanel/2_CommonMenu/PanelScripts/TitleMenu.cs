using UnityEngine;
using UnityEngine.UI;

public class TitleMenu : TitleMenuBase
{
    public void Init(Sprite background, string title)
    {
        m_Background_Image.sprite = background;
        m_Title_TMPText.text = title;

        var titleGroup = CreateWidget<TitleGroup>(m_TitleGroup_UIWidgetBehaviour);
        var settingGroup = CreateWidget<SettingGroup>(m_SettingGroup_UIWidgetBehaviour);

        titleGroup.OpenSelf();
    }

    protected override void OnClicked(Button button) { }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
    
    protected override void OnVisibleChanged(bool visible) { }
    
    protected override void OnFocusChanged(bool focus) { }
}