using System;
using System.Collections.Generic;
using UnityEngine;

public class LanguageInfos : MonoBehaviour
{
    public List<LanguageInfo> infos;
}

[Serializable]
public class LanguageInfo
{
    public SupportLanguage language;
    public Sprite flag;
    public string languageName;
}