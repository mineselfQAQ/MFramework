using MFramework.UI;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    public class MLocalizationManager : MonoSingleton<MLocalizationManager>
    {
        private SupportLanguage currentLanguage = SupportLanguage.None;
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

        private MLocalizationAsset asset;//每个ID所对应的多语言文字列表

        private Dictionary<int, List<MLocalizationInfo>> infoDic;//每个ID所对应的完整数据

        private void Awake()
        {
            LocalizationTable[] table = LocalizationTable.LoadBytes();
            asset = new MLocalizationAsset(table);

            RefreshAllText(SupportLanguage.CHINESE, true);
        }

        /// <summary>
        /// 更新InfoDic(完全刷新)
        /// Tip:由于场景中的物体是多变的(可能删除与创建)，当添加或删除时Info都会改变
        /// </summary>
        private void UpdateInfoDic()
        {
            infoDic = new Dictionary<int, List<MLocalizationInfo>>();
            List<MLocalization> localizationList = MLocalizationUtility.FindAllLoclizations();

            WriteInfo(localizationList);
        }
        public void RefreshAllText()
        {
            UpdateInfoDic();//需要更新但又没有更改语言，必然是场景数据发生变化

            foreach (var infos in infoDic.Values)
            {
                foreach (var info in infos)
                {
                    info.mText.text = info.textList[(int)CurrentLanguage];
                }
            }
        }
        public void RefreshAllText(SupportLanguage language, bool updateData = false)
        {
            if (!CheckLanguageValidity(language))
            {
                MLog.Print($"{language}未启用，不能转换为该语言", MLogType.Warning);
                return;
            }
            if (language == CurrentLanguage) return;

            if (updateData) UpdateInfoDic();

            foreach (var infos in infoDic.Values)
            {
                foreach (var info in infos)
                {
                    info.mText.text = info.textList[(int)language];
                }
            }
            CurrentLanguage = language;
        }
        private bool CheckLanguageValidity(SupportLanguage language)
        {
            if (!asset.SupportLanguages.Contains(language)) return false;
            return true;
        }
        private void WriteInfo(List<MLocalization> localizationList)
        {
            foreach (var localization in localizationList)
            {
                if (localization.LocalMode == LocalizationMode.Off || localization.LocalID == -1) continue;

                int id = localization.LocalID;
                //有对应数据
                if (asset.localDic.ContainsKey(id))
                {
                    if (!infoDic.ContainsKey(id))//第一个数据
                    {
                        List<MLocalizationInfo> infos = new List<MLocalizationInfo>();
                        MLocalizationInfo info = new MLocalizationInfo(id, asset.localDic[id], localization);
                        infos.Add(info);
                        infoDic.Add(id, infos);
                    }
                    else//更多数据(相同id的MLocalization)
                    {
                        MLocalizationInfo info = new MLocalizationInfo(id, asset.localDic[id], localization);
                        infoDic[id].Add(info);
                    }
                }
            }
        }

        /// <summary>
        /// 添加新物体并更新新物体下文字的语言
        /// </summary>
        public void AddNewText(GameObject root)
        {
            if (infoDic == null) infoDic = new Dictionary<int, List<MLocalizationInfo>>();

            List<MLocalization> localizationList = MLocalizationUtility.FindLoclizations(root);

            WriteInfo(localizationList);
            RefreshText(localizationList);
        }
        private void RefreshText(List<MLocalization> localizationList)
        {
            //TODO:!!!!!!!!!!!!!!!!!!!!!
        }
    }

    public class MLocalizationInfo
    {
        //数据
        public int id;
        public List<string> textList;
        //组件信息
        //public GameObject gameObject;
        //public MLocalization mLocal;
        public MText mText;

        public MLocalizationInfo(int id, List<string> list, MLocalization local)
        {
            this.id = id;
            textList = list;
            //gameObject = local.gameObject;
            //mLocal = local;
            mText = local.GetComponent<MText>();
        }
    }
}