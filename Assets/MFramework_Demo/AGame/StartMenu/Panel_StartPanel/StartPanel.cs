using UnityEngine;
using UnityEngine.UI;
using MFramework;

public class StartPanel : StartPanelBase
{
    private TitleBtnGroup titleBtnGroup;
    private OptionsBtnGroup optionsBtnGroup;

    public void Init(Sprite background, string titleName)
    {
        m_MBackground_MImage.sprite = background;
        m_Title_MText.text = titleName;

        titleBtnGroup = CreateWidget<TitleBtnGroup>(m_TitleBtnGroup_UIWidgetBehaviour, true);
        optionsBtnGroup = CreateWidget<OptionsBtnGroup>(m_OptionsBtnGroup_UIWidgetBehaviour);
    }

    protected override void OnClicked(Button button)
    {
        if (button == m_PointChecker_MButton)
        {
            if (optionsBtnGroup.AnimState == UIAnimState.Opened && titleBtnGroup.AnimState == UIAnimState.Closed)
            {
                CloseWidget<OptionsBtnGroup>();
                OpenWidget<TitleBtnGroup>();
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