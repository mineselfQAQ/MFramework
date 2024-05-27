using MFramework;
using System;
using System.Collections.Generic;
using System.Reflection;

public class MLocalizationAsset
{
    private LocalizationTable[] items;

    internal List<SupportLanguage> supportLanguages;

    internal Dictionary<int, List<MLocalizationString>> localDic;

    internal MLocalizationAsset(LocalizationTable[] table)
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
        supportLanguages = new List<SupportLanguage>();
        foreach (var property in properties)
        {
            if (supportLanguageDic.ContainsKey(property.Name))//是类型中的一种
            {
                supportLanguages.Add(supportLanguageDic[property.Name]);
            }
        }

        //根据excel表建立|id--->各语言详情|的映射
        localDic = new Dictionary<int, List<MLocalizationString>>();
        foreach (var item in items)
        {
            if (localDic.ContainsKey(item.ID)) continue;

            List<MLocalizationString> textList = new List<MLocalizationString>();
            for (int i = 0; i < supportLanguages.Count; i++)
            {
                SupportLanguage language = supportLanguages[i];
                var info = type.GetProperty(language.ToString());
                string text = (string)info.GetValue(item);
                textList.Add(new MLocalizationString(text, language));
            }

            localDic.Add(item.ID, textList);
        }
    }
}

public enum SupportLanguage
{
    CHINESE = 0,
    ENGLISH = 1,
    JAPANESE = 2,

    None = 1000
}