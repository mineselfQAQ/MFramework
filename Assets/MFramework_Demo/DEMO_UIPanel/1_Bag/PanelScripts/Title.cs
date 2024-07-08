using UnityEngine.UI;

public class Title : TitleBase
{
    public void Init(int id, string name)
    {
        m_ID_TMPText.text = $"ID:{id}";
        m_Name_TMPText.text = $"Bag{name}";
    }

    protected override void OnClicked(Button button) { }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
}