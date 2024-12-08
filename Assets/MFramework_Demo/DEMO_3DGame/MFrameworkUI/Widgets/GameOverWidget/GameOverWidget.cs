using UnityEngine;
using UnityEngine.UI;

public class GameOverWidget : GameOverWidgetBase
{
    public override void Init()
    {
        
    }

    protected override GameObject LoadPrefab(string prefabPath)
    {
        return ABUtility.LoadPanelSync(prefabPath);
    }
}