using UnityEngine.UI;

public class SettingWidget : SettingWidgetBase
{
    public override void Init()
    {
        
    }

    public override void Update()
    {
        
    }

    protected override void OnClicked(Button button) 
    {
        if (button == m_LanguageBtn_MButton)
        {

        }
        else if (button == m_CloseBtn_MButton)
        {

        }
    }

    protected override void OnValueChanged(Slider slider, float value)
    {
        if (slider == m_SFXSlider_Slider)
        {

        }
        else if (slider == m_MusicSlider_Slider)
        {

        }
    }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
    
    
}