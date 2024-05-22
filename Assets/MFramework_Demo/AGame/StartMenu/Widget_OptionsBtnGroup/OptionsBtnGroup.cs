using MFramework;
using MFramework.UI;
using System.Collections.Generic;
using Table;
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
            string info = GetOptionInfo(m_SoundText_MText.text, "Sound:");
            if (info == "On")
            {
                AudioListener.volume = 0;
                m_SoundText_MText.text = "Sound:Off";
            }
            else
            {
                AudioListener.volume = 1;
                m_SoundText_MText.text = "Sound:On";
            }
        }
        else if (button == m_LanguageBtn_MButton)
        {
            string info = GetOptionInfo(m_LanguageText_MText.text, "Language:");
            if (info == "Chinese")
            {
                var table = LocalizationTable.LoadBytes();
                var loclizations = FindAllLoclization();
                foreach (var loc in loclizations)
                {
                    foreach (var item in table)
                    {
                        if (item.ID == loc.localID)
                        {
                            loc.GetComponent<MText>().text = item.ENGLISH;
                        }
                    }
                }
                m_LanguageText_MText.text = "Language:English";
            }
            else if (info == "English")
            {
                var table = LocalizationTable.LoadBytes();
                var loclizations = FindAllLoclization();
                foreach (var loc in loclizations)
                {
                    foreach (var item in table)
                    {
                        if (item.ID == loc.localID)
                        {
                            loc.GetComponent<MText>().text = item.CHINESE;
                        }
                    }
                }
                m_LanguageText_MText.text = "Language:Chinese";
            }
        }
    }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
    
    private string GetOptionInfo(string fullText, string prefix)
    {
        string text = fullText.Substring(prefix.Length);

        return text;
    }

    private List<MLocalization> FindAllLoclization()
    {
        List<MLocalization> list = new List<MLocalization>(GameObject.FindObjectsOfType<MLocalization>(true));
        return list;
    }
}