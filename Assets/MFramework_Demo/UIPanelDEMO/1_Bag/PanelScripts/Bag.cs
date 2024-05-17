using UnityEngine;
using UnityEngine.UI;

public class Bag : BagBase
{
    private int id;

    public void Init(int id, Color color, Vector2 size)
    {
        this.id = id;
        m_Background_Image.color = color;
        m_Bag_RectTransform.sizeDelta = size;

        var title = CreateWidget<Title>("Title", trans, @"Assets\MFramework_Demo\UIPanelDEMO\1_Bag\Prefab\Title.prefab");
        title.Init(id, Random.Range(0, 100).ToString());
    }

    protected override void OnClicked(Button button) { }

    protected override void OnCreating() { }

    protected override void OnCreated() 
    {
        //SetVisibleSelf(false);
        CloseSelf();
    }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
    
    protected override void OnVisibleChanged(bool visible) { }
    
    protected override void OnFocusChanged(bool focus) { }
}