using MFramework.UI;
using System;
using System.Collections.Generic;
using UnityEditor.VersionControl;
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

        public List<SupportLanguage> SupportLanguages => asset.supportLanguages;
        public int SupportLanguagesCount => asset.supportLanguages.Count;

        private MLocalizationAsset asset;//每个ID所对应的多语言文字列表

        internal Dictionary<int, List<MLocalizationInfo>> infoDic;//每个ID所对应的完整数据

        private void Awake()
        {
            LocalizationTable[] table = LocalizationTable.LoadBytes();
            asset = new MLocalizationAsset(table);
            UpdateAllText(SupportLanguage.CHINESE, true);
        }

        public int GetCurState(MText text, int pos)
        {
            if (!infoDic.ContainsKey(text.mLocal.LocalID)) return -1;

            List<MLocalizationInfo> infoList = infoDic[text.mLocal.LocalID];
            var info = infoList[0];
            return info.textList[0].formatStrList[pos].curState;
        }

        public void ChangeState(MText text, int pos, int state)
        {
            if (!infoDic.ContainsKey(text.mLocal.LocalID)) return;

            List<MLocalizationInfo> infoList = infoDic[text.mLocal.LocalID];
            foreach (var info in infoList)
            {
                foreach (var t in info.textList)
                {
                    t.ChangeState(pos, state);
                }
            }

            UpdateText(text);
        }
        //public void ChangeState(int id, int pos, int state)
        //{
        //    if (!infoDic.ContainsKey(id)) return;

        //    List<MLocalizationInfo> infoList = infoDic[id];
        //    foreach (var info in infoList)
        //    {
        //        foreach (var t in info.textList)
        //        {
        //            t.ChangeState(pos, state);
        //        }
        //    }

        //    UpdateAllText();//TODO:太耗
        //}

        public void UpdateAllText()
        {
            UpdateAllInfo();//需要更新但又没有更改语言，必然是场景数据发生变化

            foreach (var infos in infoDic.Values)
            {
                foreach (var info in infos)
                {
                    info.mText.text = info.textList[(int)CurrentLanguage].ToString();
                }
            }
        }
        public void UpdateAllText(SupportLanguage language, bool updateData = false)
        {
            if (!CheckLanguageValidity(language))
            {
                MLog.Print($"{language}未启用，不能转换为该语言", MLogType.Warning);
                return;
            }
            if (language == CurrentLanguage) return;

            if (updateData) UpdateAllInfo();

            foreach (var infos in infoDic.Values)
            {
                foreach (var info in infos)
                {
                    info.mText.text = info.textList[(int)language].ToString();
                }
            }
            CurrentLanguage = language;
        }

        public void UpdateText(MText text)
        {
            if (infoDic == null) infoDic = new Dictionary<int, List<MLocalizationInfo>>();

            List<MLocalization> localizationList = new List<MLocalization>() { text.mLocal };

            UpdateInfo(localizationList, (info) => { info.mText.text = info.textList[(int)CurrentLanguage].ToString(); });
        }

        /// <summary>
        /// 添加新物体并更新新物体下文字的语言
        /// </summary>
        public void UpdateNewText(GameObject root)
        {
            if (infoDic == null) infoDic = new Dictionary<int, List<MLocalizationInfo>>();

            List<MLocalization> localizationList = MLocalizationUtility.FindLoclizations(root);

            UpdateInfoAndUpdateView(localizationList);
        }

        /// <summary>
        /// 更新InfoDic(完全刷新)
        /// Tip:由于场景中的物体是多变的(可能删除与创建)，当添加或删除时Info都会改变
        /// </summary>
        private void UpdateAllInfo()
        {
            infoDic = new Dictionary<int, List<MLocalizationInfo>>();
            List<MLocalization> localizationList = MLocalizationUtility.FindAllLoclizations();

            UpdateInfo(localizationList);
        }
        /// <summary>
        /// 将信息写入infoDic
        /// </summary>
        /// <param name="localizationList"></param>
        private void UpdateInfo(List<MLocalization> localizationList, Action<MLocalizationInfo> onFindInfo = null)
        {
            foreach (var localization in localizationList)
            {
                if (localization.LocalMode == LocalizationMode.Off || localization.LocalID == -1) continue;

                int id = localization.LocalID;
                //有对应数据
                if (asset.localDic.ContainsKey(id))
                {
                    MLocalizationInfo info;
                    if (!infoDic.ContainsKey(id))//第一个数据
                    {
                        List<MLocalizationInfo> infos = new List<MLocalizationInfo>();
                        info = new MLocalizationInfo(id, asset.localDic[id], localization);
                        infos.Add(info);
                        infoDic.Add(id, infos);
                    }
                    else//更多数据(相同id的MLocalization)
                    {
                        info = new MLocalizationInfo(id, asset.localDic[id], localization);
                        infoDic[id].Add(info);
                    }
                    onFindInfo?.Invoke(info);
                }
            }
        }
        private void UpdateInfoAndUpdateView(List<MLocalization> localizationList)
        {
            UpdateInfo(localizationList, (info) => { info.mText.text = info.textList[(int)CurrentLanguage].ToString(); });
        }

        private bool CheckLanguageValidity(SupportLanguage language)
        {
            if (!SupportLanguages.Contains(language)) return false;
            return true;
        }
    }

    public class MLocalizationInfo
    {
        //数据
        public int id;
        public List<MLocalizationString> textList;//多语言
        //组件信息
        //public GameObject gameObject;
        //public MLocalization mLocal;
        public MText mText;

        public MLocalizationInfo(int id, List<MLocalizationString> list, MLocalization local)
        {
            this.id = id;
            textList = list;
            //gameObject = local.gameObject;
            //mLocal = local;
            mText = local.GetComponent<MText>();
        }
    }
}