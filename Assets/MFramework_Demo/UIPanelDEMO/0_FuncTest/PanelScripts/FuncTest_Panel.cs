using UnityEngine.UI;

public class FuncTest_Panel : FuncTest_PanelBase
{
    public void Init()
    {
        var widget11 = CreateWidget<FuncTest_Widget1>("Widget11", m_Widget1Group_RectTransform, @"Assets\MFramework_Demo\UIPanelDEMO\0_FuncTest\Prefab\FuncTest_Widget1.prefab", true);
        var widget12 = CreateWidget<FuncTest_Widget1>("Widget12", m_Widget1Group_RectTransform, @"Assets\MFramework_Demo\UIPanelDEMO\0_FuncTest\Prefab\FuncTest_Widget1.prefab", true);
        var widget13 = CreateWidget<FuncTest_Widget1>("Widget13", m_Widget1Group_RectTransform, @"Assets\MFramework_Demo\UIPanelDEMO\0_FuncTest\Prefab\FuncTest_Widget1.prefab", true);
        var widget21 = CreateWidget<FuncTest_Widget2>("Widget21", m_FuncTest_Widget2_UIWidgetBehaviour);
        widget21.Init();
        widget21.Open();
    }

    protected override void OnClicked(Button button) { }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
    
    protected override void OnVisibleChanged(bool visible) { }
    
    protected override void OnFocusChanged(bool focus) { }
}