using MFramework.UI;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// 目前架构：
    /// infoDic---存放id下的所有信息，一个id可能有多个信息
    /// MLocalizationInfo---完整信息，拥有多种语言
    /// </summary>
    [MonoSingletonSetting(HideFlags.NotEditable, "#MLocalizationManager#")]
    public class MLocalizationManager : MonoSingleton<MLocalizationManager>
    {
        private SupportLanguage currentLanguage = SupportLanguage.Default;
        public SupportLanguage CurrentLanguage
        {
            internal set
            {
                currentLanguage = value;
            }
            get
            {
                return currentLanguage;
            }
        }
        public List<SupportLanguage> SupportLanguages => asset.supportLanguages;
        public int SupportLanguagesCount => asset.supportLanguages.Count;

        internal MLocalizationAsset asset;//每个ID所对应的多语言文字列表

        private void Awake()
        {
            LocalizationTable[] table = LocalizationTable.LoadBytes();
            asset = new MLocalizationAsset(table);

            InitCurrentLanguage();
        }

        private void InitCurrentLanguage()
        {
            string language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            if (language == "zh") 
            {
                currentLanguage = SupportLanguage.CHINESE;
            }
            else if (language == "en")
            {
                currentLanguage = SupportLanguage.ENGLISH;
            }
            //TODO:扩充其它语言
        }

        public void SetLanguage(SupportLanguage language)
        {
            if (!CheckLanguageValidity(language))
            {
                MLog.Print($"{typeof(MLocalizationManager)}：{language}未启用，不能转换为该语言，请检查", MLogType.Warning);
                return;
            }
            if (language == currentLanguage) return;

            currentLanguage = language;

            MText.UpdateAllInfo();
        }

        private bool CheckLanguageValidity(SupportLanguage language)
        {
            if (!SupportLanguages.Contains(language)) return false;
            return true;
        }
    }
}