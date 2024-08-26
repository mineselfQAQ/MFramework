using MFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingWidget : SettingWidgetBase
{
    protected int i;
    protected List<LanguageInfo> infos;

    protected SoundController sound = SoundController.Instance;

    public override void Init()
    {
        //语言初始化
        infos = m_SettingWidget_LanguageInfos.infos;
        for (int j = 0; j < infos.Count; j++)
        {
            if (infos[j].language == MLocalizationManager.Instance.CurrentLanguage)
            {
                i = j;
                break;
            }
        }
        RefreshLanguageView();

        //音量初始化
        m_SFXSlider_Slider.value = sound.curSFX;
        m_MusicSlider_Slider.value = sound.curMusic;
    }

    protected override void OnClicked(Button button) 
    {
        if (button == m_LanguageBtn_MButton)
        {
            i = (i + 1) % infos.Count;
            MLocalizationManager.Instance.SetLanguage(infos[i].language);
            RefreshLanguageView();
        }
        else if (button == m_CloseBtn_MButton)
        {
            CloseSelf();
            sound.SaveSoundJson();
        }
    }

    protected override void OnValueChanged(Slider slider, float value)
    {
        if (slider == m_SFXSlider_Slider)
        {
            sound.mixer.SetFloat(sound.SFX, MMath.LinearToDecibel(value));
            sound.curSFX = value;
        }
        else if (slider == m_MusicSlider_Slider)
        {
            sound.mixer.SetFloat(sound.MUSIC, MMath.LinearToDecibel(value));
            sound.curMusic = value;
        }
    }

    protected void RefreshLanguageView()
    {
        m_LanguageIcon_MImage.sprite = infos[i].flag;
        m_LanguageText_MText.text = infos[i].languageName;
    }
}