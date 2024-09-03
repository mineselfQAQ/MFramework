using MFramework;
using UnityEngine;

public static class ABUtitlity
{
    public static GameObject LoadPanelSync(string path)
    {
        IResource panelResource = ResourceManager.Instance.Load(path, false);
        return panelResource.GetAsset<GameObject>();
    }
}
