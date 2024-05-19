using UnityEngine.UI;

public class FuncTest_Widget2 : FuncTest_Widget2Base
{
    public void Init()
    {
        var childWidget = CreateWidget<FuncTest_ChildWidget>("ChildWidget", rectTransform, @"Assets\MFramework_Demo\UIPanelDEMO\0_FuncTest\Prefab\FuncTest_ChildWidget.prefab");
    }

    protected override void OnClicked(Button button) { }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
    
    
}