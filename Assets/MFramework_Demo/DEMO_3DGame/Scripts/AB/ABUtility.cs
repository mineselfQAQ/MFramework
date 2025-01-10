using MFramework;
using UnityEngine;

public static class ABUtility
{
    public static GameObject LoadPanelSync(string path)
    {
        if (!MCore.Instance.ABState)
        {
            MLog.Print("�Զ������UI����ʹ��AB���뿪��Mcore�е�ABState", MLogType.Error);
        }

        IResource panelResource = MResourceManager.Instance.Load(path, false);
        return panelResource.GetAsset<GameObject>();
    }
}
