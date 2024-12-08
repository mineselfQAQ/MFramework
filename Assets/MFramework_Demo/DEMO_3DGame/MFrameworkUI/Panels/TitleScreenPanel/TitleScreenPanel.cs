using MFramework;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenPanel : TitleScreenPanelBase
{
    protected override void OnClicked(Button button) 
    {
        if (button == m_StartBtn_MButton)
        {
            UIController.Instance.TitleScreenToFileSelect();
        }
    }

    protected override GameObject LoadPrefab(string prefabPath)
    {
        return ABUtility.LoadPanelSync(prefabPath);
    }
}