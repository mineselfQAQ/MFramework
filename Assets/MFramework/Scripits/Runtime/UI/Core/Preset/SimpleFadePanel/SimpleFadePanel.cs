using MFramework;
using UnityEngine;

public class SimpleFadePanel : SimpleFadePanelBase
{
    protected override GameObject LoadPrefab(string prefabPath)
    {
        IResource panelResource = MResourceManager.Instance.Load(prefabPath, false);
        return panelResource.GetAsset<GameObject>();
    }
}