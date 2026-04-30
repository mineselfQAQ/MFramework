using UnityEngine;
using UnityEngine.UI;

public class MPanel : MPanelBase
{
    protected override void OnClicked(Button button)
    {
    }

    protected override GameObject LoadPrefab(string prefabPath)
    {
        return base.LoadPrefab(prefabPath);
    }

    protected override void OnCreating() { }
    protected override void OnCreated() { }
    protected override void OnDestroying() { }
    protected override void OnDestroyed() { }

    protected override void OnVisibleChanged(bool visible) { }
    protected override void OnFocusChanged(bool focus) { }
}