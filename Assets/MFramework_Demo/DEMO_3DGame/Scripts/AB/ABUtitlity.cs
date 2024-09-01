using MFramework;
using UnityEngine;

public static class ABUtitlity
{
    public static GameObject LoadPanelSync(string path)
    {
        IResource panelResource = ResourceManager.Instance.Load($"{ABPath.MFRAMEWORKUI}/Panels/{path}", false);
        return panelResource.GetAsset<GameObject>();
    }

    public static GameObject LoadWidgetSync(string path)
    {
        IResource panelResource = ResourceManager.Instance.Load($"{ABPath.MFRAMEWORKUI}/Widgets/{path}", false);
        return panelResource.GetAsset<GameObject>();
    }

    public static GameObject LoadSync(string path)
    {
        IResource panelResource = ResourceManager.Instance.Load(path, false);
        return panelResource.GetAsset<GameObject>();
    }
}
