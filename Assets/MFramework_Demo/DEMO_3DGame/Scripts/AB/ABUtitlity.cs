using MFramework;
using UnityEngine;

public static class ABUtitlity
{
    public static GameObject LoadPanelSync(string path)
    {
        if (!ABController.Instance.enableAB)
        {
            MLog.Print("�Զ������UI����ʹ��AB���뿪��ABController�е�enableAB", MLogType.Error);
        }

        IResource panelResource = MResourceManager.Instance.Load(path, false);
        return panelResource.GetAsset<GameObject>();
    }
}
