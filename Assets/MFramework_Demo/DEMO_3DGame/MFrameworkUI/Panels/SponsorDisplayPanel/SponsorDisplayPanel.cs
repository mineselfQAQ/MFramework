using UnityEngine;

public class SponsorDisplayPanel : SponsorDisplayPanelBase
{
    protected override GameObject LoadPrefab(string prefabPath)
    {
        return ABUtility.LoadPanelSync(prefabPath);
    }
}