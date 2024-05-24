using MFramework;
using MFramework.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsBtnGroup : OptionsBtnGroupBase
{
    public void Init()
    {

    }

    protected override void OnClicked(Button button)
    {
        if (button == m_SoundBtn_MButton)
        {
            if (m_SoundText_MText.text.Contains("On"))
            {
                AudioListener.volume = 0;
                m_SoundText_MText.text = m_SoundText_MText.text.Replace("On", "Off");
            }
            else
            {
                AudioListener.volume = 1;
                m_SoundText_MText.text = m_SoundText_MText.text.Replace("Off", "On");
            }
        }
        else if (button == m_LanguageBtn_MButton)
        {
            if (MLocalizationManager.Instance.CurrentLanguage == SupportLanguage.CHINESE)
            {
                MLocalizationManager.Instance.RefreshAllText(SupportLanguage.ENGLISH);
            }
            else if (MLocalizationManager.Instance.CurrentLanguage == SupportLanguage.ENGLISH)
            {
                MLocalizationManager.Instance.RefreshAllText(SupportLanguage.CHINESE);
            }
        }
    }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
}