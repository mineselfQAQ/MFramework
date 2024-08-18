using MFramework.UI;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// Ŀǰ�ܹ���
    /// infoDic---���id�µ�������Ϣ��һ��id�����ж����Ϣ
    /// MLocalizationInfo---������Ϣ��ӵ�ж�������
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

        internal MLocalizationAsset asset;//ÿ��ID����Ӧ�Ķ����������б�

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
            //TODO:������������
        }

        public void SetLanguage(SupportLanguage language)
        {
            if (!CheckLanguageValidity(language))
            {
                MLog.Print($"{typeof(MLocalizationManager)}��{language}δ���ã�����ת��Ϊ�����ԣ�����", MLogType.Warning);
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