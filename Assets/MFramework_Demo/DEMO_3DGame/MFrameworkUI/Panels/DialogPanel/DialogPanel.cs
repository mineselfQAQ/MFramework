using UnityEngine;
using UnityEngine.UI;

public class DialogPanel : DialogPanelBase
{
    public override void Init()
    {
        m_ProfilePhoto_MImage.sprite = null;
        m_Name_MText.text = "Null";
        m_Message_MText.text = "";
    }

    public void RefreshView(Sprite sprite, string name, string message)
    {
        m_ProfilePhoto_MImage.sprite = sprite;
        m_Name_MText.text = name;
        m_Message_MText.text = message;
    }

    protected override void OnClicked(Button button) 
    {
        
    }

    protected override GameObject LoadPrefab(string prefabPath)
    {
        return ABUtitlity.LoadPanelSync(prefabPath);
    }
}