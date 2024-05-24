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

    internal Dictionary<int, List<string>> localDic;

    public MLocalizationAsset(LocalizationTable[] table)
    {
        if (table == null || table.Length == 0)
        {
            MLog.Print($"MLocalization：xlsx表不存在或无数据，请检查", MLogType.Warning);
            return;
        }

        items = table;

        Dictionary<string, SupportLanguage> supportLanguageDic = new Dictionary<string, SupportLanguage>()
        { 
            { "CHINESE" , SupportLanguage.CHINESE  },
            { "ENGLISH" , SupportLanguage.ENGLISH  },
            { "JAPENESE", SupportLanguage.JAPANESE } 
        };
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
    }
}

public enum SupportLanguage
{
    CHINESE = 0,
    ENGLISH = 1,
    JAPANESE = 2,

    None = 1000
}