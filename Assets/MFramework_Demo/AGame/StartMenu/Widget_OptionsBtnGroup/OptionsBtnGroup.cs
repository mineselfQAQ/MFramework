using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;

public class OptionsBtnGroup : OptionsBtnGroupBase
{
    protected override void OnClicked(Button button)
    {
        if (button == m_SoundBtn_MButton)
        {
            if (m_SoundText_MText.GetCurState(0) == 0)//开--->关
            {
                AudioListener.volume = 0;
                m_SoundText_MText.ChangeState(0, 1);
            }
            else//关--->开
            {
                AudioListener.volume = 1;
                m_SoundText_MText.ChangeState(0, 0);
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