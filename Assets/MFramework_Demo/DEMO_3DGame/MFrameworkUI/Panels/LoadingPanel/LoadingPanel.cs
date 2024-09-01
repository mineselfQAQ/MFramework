using MFramework;
using UnityEngine;

public class LoadingPanel : LoadingPanelBase
{
    protected override GameObject LoadPrefab(string prefabPath)
    {
        return ABUtitlity.LoadSync(prefabPath);
    }
}