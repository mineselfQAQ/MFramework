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
                //TODO:!!!!
            }
            else
            {
                AudioListener.volume = 1;
            }
        }
        else if (button == m_LanguageBtn_MButton)
        {
            if (MLocalizationManager.Instance.CurrentLanguage == SupportLanguage.CHINESE)
            {
                MLocalizationManager.Instance.UpdateAllText(SupportLanguage.ENGLISH);
            }
            else if (MLocalizationManager.Instance.CurrentLanguage == SupportLanguage.ENGLISH)
            {
                MLocalizationManager.Instance.UpdateAllText(SupportLanguage.CHINESE);
            }
        }
    }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
}