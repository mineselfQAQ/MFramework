using MFramework;
using UnityEngine;

public static class ABUtitlity
{
    public static GameObject LoadPanelSync(string path)
    {
        if (!ABController.Instance.enableAB)
        {
            MLog.Print("自定义加载UI必须使用AB，请开启ABController中的enableAB", MLogType.Error);
        }

        IResource panelResource = MResourceManager.Instance.Load(path, false);
        return panelResource.GetAsset<GameObject>();
    }
}
