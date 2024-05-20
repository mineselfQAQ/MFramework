using UnityEngine.UI;

public class SettingGroup : SettingGroupBase
{
    public void Init()
    {
        
    }

    protected override void OnClicked(Button button) 
    {
        if (button == m_LanguageBtn_Button)
        {
            if (m_LanguageText_Text.text == "Language:Chinese")
            {
                m_LanguageText_Text.text = "Language:English";
            }
            else if (m_LanguageText_Text.text == "Language:English")
            {
                m_LanguageText_Text.text = "Language:Chinese";
            }
        }

        if (button == m_BackBtn_Button)
        {
            CloseSelf();
            parentView.OpenWidget<TitleGroup>();
        }
    }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
    
    
}