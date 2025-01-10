using MFramework;
using UnityEngine;

public static class ABUtility
{
    public static GameObject LoadPanelSync(string path)
    {
        if (!MCore.Instance.ABState)
        {
            MLog.Print("自定义加载UI必须使用AB，请开启Mcore中的ABState", MLogType.Error);
        }

        IResource panelResource = MResourceManager.Instance.Load(path, false);
        return panelResource.GetAsset<GameObject>();
    }
}
