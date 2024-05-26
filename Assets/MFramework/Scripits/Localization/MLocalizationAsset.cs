using MFramework;
using System;
using System.Collections.Generic;
using System.Reflection;

public class MLocalizationAsset
{
    private LocalizationTable[] items;

    private List<SupportLanguage> supportLanguages;
    public List<SupportLanguage> SupportLanguages
    {
        private set { supportLanguages = value; }
        get { return supportLanguages; }
    }

    public int SupportLanguageCount => supportLanguages.Count;

    internal Dictionary<int, List<string>> localDic;//TODO:更改为List<MLocalizationString>

    public MLocalizationAsset(LocalizationTable[] table)
    {
        if (table == null || table.Length == 0)
        {
            MLog.Print($"MLocalization：未获取到xlsx表在或xlsx表内无数据，请检查", MLogType.Warning);
            return;
        }

        items = table;

        //可用语言选项
        Dictionary<string, SupportLanguage> supportLanguageDic = new Dictionary<string, SupportLanguage>()
        { 
            { "CHINESE" , SupportLanguage.CHINESE  },
            { "ENGLISH" , SupportLanguage.ENGLISH  },
            { "JAPENESE", SupportLanguage.JAPANESE } 
        };
        //获取可用语言
        Type type = typeof(LocalizationTable);
        PropertyInfo[] properties = type.GetProperties();
        SupportLanguages = new List<SupportLanguage>();
        foreach (var property in properties)
        {
            if (supportLanguageDic.ContainsKey(property.Name))//是类型中的一种
            {
                supportLanguages.Add(supportLanguageDic[property.Name]);
            }
        }
        //根据excel表建立|id--->各语言详情|的映射
        localDic = new Dictionary<int, List<string>>();
        foreach (var item in items)
        {
            if (localDic.ContainsKey(item.ID)) continue;

            List<string> textList = new List<string>();
            for (int i = 0; i < SupportLanguageCount; i++)
            {
                var info = type.GetProperty(SupportLanguages[i].ToString());
                string text = (string)info.GetValue(item);
                textList.Add(text);
            }

            localDic.Add(item.ID, textList);
        }

        //此时应该获取到：
        //5---|音量:{开|关}| |Sound:{On|Off}|
        //需要继续处理：
        //以中文为例：|音量:|+|开|
        //List<string>
        //Add("音量:") Add("开"or"关")
        //ChangeState()--->将"开"替换为"关"
    }
}

public enum SupportLanguage
{
    CHINESE = 0,
    ENGLISH = 1,
    JAPANESE = 2,

    None = 1000
}