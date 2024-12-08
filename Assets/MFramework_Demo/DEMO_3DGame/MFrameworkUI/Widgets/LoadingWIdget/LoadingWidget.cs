using UnityEngine;
using UnityEngine.UI;

public class LoadingWidget : LoadingWidgetBase
{
    public override void Init()
    {

    }

    protected override GameObject LoadPrefab(string prefabPath)
    {
        return ABUtility.LoadPanelSync(prefabPath);
    }
}