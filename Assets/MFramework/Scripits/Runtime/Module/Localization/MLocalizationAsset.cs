using MFramework;
using System;
using System.Collections.Generic;
using System.Reflection;

public class MLocalizationAsset
{
    /// <summary>
    /// Excel本表
    /// </summary>
    internal LocalizationTable[] items;

    /// <summary>
    /// 支持语言种类列表
    /// </summary>
    internal List<SupportLanguage> supportLanguages;

    /// <summary>
    /// Excel表格拆分(每个id一组信息(横向拆分))
    /// </summary>
    internal Dictionary<int, LocalizationTable> tableDic;

    //可用语言选项
    Dictionary<string, SupportLanguage> supportLanguageDic = new Dictionary<string, SupportLanguage>()
    {
        { "CHINESE" , SupportLanguage.CHINESE  },
        { "ENGLISH" , SupportLanguage.ENGLISH  },
        { "JAPENESE", SupportLanguage.JAPANESE }
    };

    internal MLocalizationAsset(LocalizationTable[] table)
    {
        if (table == null || table.Length == 0)
        {
            MLog.Print($"{typeof(MLocalizationAsset)}：未获取到xlsx表在或xlsx表内无数据，请检查", MLogType.Error);
            return;
        }

        items = table;
        //建立tableDic
        tableDic = new Dictionary<int, LocalizationTable>();
        foreach (var item in items)
        {
            if (!tableDic.ContainsKey(item.ID))//确保key不重复
            {
                tableDic.Add(item.ID, item);
            }
        }

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
    }
}

public enum SupportLanguage
{
    CHINESE = 0,
    ENGLISH = 1,
    JAPANESE = 2,

    Default = 1000
}