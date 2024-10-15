using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MFramework.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MLocalization))]
    [AddComponentMenu("MFramework/MText")]
    public class MText : TextMeshProUGUI
    {
        public static List<MText> textList = new List<MText>();//所有的MText列表

        public MLocalization mLocal;
        public MTextAnimator mAnimator;

        internal Dictionary<SupportLanguage, ParsedText> parsedTextDic;//经过格式化的字符串(根据语言获取)
        //初步判断内联
        internal bool enableLocalization = false;
        internal bool enableTextAnimatorInline = false;

        protected override void Awake()
        {
            base.Awake();

            //父类使用了[ExecuteAlways]，需要防止编辑器下调用，如MLocalizationManager.Instance.asset.tableDic就无法在编辑器模式下获取
            if (Application.isPlaying)
            {
                //延迟一帧
                //MCoroutineManager.Instance.DelayOneFrame(() => Init());
                Init();
            }
        }
        private void Init()
        {
            textList.Add(this);

            mLocal = GetComponent<MLocalization>();//必定存在
            mAnimator = GetComponent<MTextAnimator>();//可有可无
            mAnimator?.Init();

            if (mLocal.LocalMode == LocalizationMode.On && mLocal.LocalID != -1)
            {
                //有可能存在本地化内联(不一定，是可选内容)
                enableLocalization = true;
            }
            if (mAnimator != null && mAnimator.inlineEffectsSwitch == InlineEffectSwitch.On)
            {
                //有可能存在动画内联(不一定，只是启用了而已)
                enableTextAnimatorInline = true;
            }

            if (enableLocalization || enableTextAnimatorInline)
            {
                //Tip:
                //开启本地化---存在key-语言 value-解析字符串的字典(多种语言)
                //不开启本地化---存在key-语言 value-解析字符串的字典(1种语言-Default)
                //是否开启文件动画会体现在ParsedText之中
                parsedTextDic = GenerateParsedTextDic();
                UpdateView();
            }
        }
        internal Dictionary<SupportLanguage, ParsedText> GenerateParsedTextDic()
        {
            Dictionary<SupportLanguage, ParsedText> dic = null;
            if (enableLocalization)//开启本地化
            {
                var tableDic = MLocalizationManager.Instance.asset.tableDic;
                int id = mLocal.LocalID;
                if (tableDic.ContainsKey(id))
                {
                    var table = tableDic[id];
                    dic = new Dictionary<SupportLanguage, ParsedText>();
                    TableToParsedTextDic(table, dic);
                }
            }
            else//未开启本地化
            {
                //此时只应该有一种语言，那么只能是SupportLanguage.Default
                //同时，由于没有开启本地化，那么一定只有内联开启了才会进入
                dic = new Dictionary<SupportLanguage, ParsedText>();
                ParsedText parsedText = new ParsedText(text, SupportLanguage.Default);//没有本地化时只可能使用MText中赋值的text
                dic.Add(SupportLanguage.Default, parsedText);
            }
            return dic;
        }
        private void TableToParsedTextDic(LocalizationTable table, Dictionary<SupportLanguage, ParsedText> dic)
        {
            int n = MLocalizationManager.Instance.SupportLanguagesCount;
            for (int i = 0; i < n; i++)
            {
                SupportLanguage lanaguage = MLocalizationManager.Instance.SupportLanguages[i];
                Type type = typeof(LocalizationTable);
                var info = type.GetProperty(lanaguage.ToString());//获取需要的语言(PropertyInfo)
                string text = (string)info.GetValue(table);//通过实例获取存储的文字

                dic.Add(lanaguage, new ParsedText(text, lanaguage));
            }
        }

        protected override void Start()
        {
            base.Start();

            if (Application.isPlaying)
            {
                PlayAnimator();
            }
        }
        private void PlayAnimator()
        {
            ParsedText pText = GetCurParsedText();
            mAnimator?.PlayInit(pText);
        }
        internal ParsedText GetCurParsedText()
        {
            if (parsedTextDic == null) return null;

            if (parsedTextDic.ContainsKey(SupportLanguage.Default))//Default情况
            {
                return parsedTextDic[SupportLanguage.Default];
            }
            else
            {
                return parsedTextDic[MLocalizationManager.Instance.CurrentLanguage];
            }
        }

        #region 文字动画
        public void PlayText()
        {
            if (mAnimator == null)
            {
                MLog.Print($"{typeof(MText)}：{name}中没有添加MTextAnimator组件，请添加后尝试调用打字机效果", MLogType.Warning);
                return;
            }

            mAnimator.PlayText();
        }

        public void PlayTextFastly()
        {
            if (mAnimator == null)
            {
                MLog.Print($"{typeof(MText)}：{name}中没有添加MTextAnimator组件，请添加后再尝试", MLogType.Warning);
                return;
            }

            mAnimator.PlayTextFastly();
        }

        public void FinishTextImmediately()
        {
            if (mAnimator == null)
            {
                MLog.Print($"{typeof(MText)}：{name}中没有添加MTextAnimator组件，请添加后再尝试", MLogType.Warning);
                return;
            }

            mAnimator.FinishTextImmediately();
        }
        #endregion

        #region 数据更新
        public static void UpdateAllInfo()
        {
            foreach (var text in textList)
            {
                text.UpdateInfo();
            }
        }

        internal void UpdateInfo()
        {
            if (parsedTextDic == null) return;

            //如果出现内联，说明有可能需要更新
            //bool haveAnimatorInline = false, haveLocalInline = false;
            //foreach (var parsedText in parsedTextDic.Values)
            //{
            //    haveAnimatorInline = parsedText.haveAnimatorInline;
            //    haveLocalInline = parsedText.haveLocalInline;
            //    break;
            //}

            //开启本地化就有可能需要更新
            if (enableLocalization)
            {
                //将所有的ParsedText进行更新(因为本地化内联更换选项)
                foreach (var parsedText in parsedTextDic.Values)
                {
                    parsedText.UpdateData();
                }
                UpdateView();//更新视图
            }
        }

        internal void UpdateView()
        {
            //2种更新的可能：
            //1.语言切换
            //2.本地化内联更换选项

            SupportLanguage curLanguage = MLocalizationManager.Instance.CurrentLanguage;
            if (parsedTextDic.ContainsKey(SupportLanguage.Default))
            {
                text = parsedTextDic[SupportLanguage.Default].ToString();
            }
            else if (parsedTextDic.ContainsKey(curLanguage))
            {
                text = parsedTextDic[curLanguage].ToString();
            }
        }
        #endregion

        public void UpdateParsedTextDic(int id)
        {
            mLocal.LocalID = id;
            parsedTextDic = GenerateParsedTextDic();
            UpdateView();
        }
    }

    public static class MTextExtension
    {
        public static void ChangeState(this MText text, int pos, int state)
        {
            foreach (var parsedText in text.parsedTextDic.Values)
            {
                parsedText.ChangeLocalState(pos, state);
            }

            text.UpdateInfo();
        }

        public static int GetCurState(this MText text, int pos)
        {
            foreach (var parsedText in text.parsedTextDic.Values)
            {
                return parsedText.GetCurState(pos);
            }
            return -1;
        }

        /// <summary>
        /// 根据传入Alpha值更改
        /// </summary>
        public static void SetAlpha(this TMP_Text text, float alpha)
        {
            alpha = Mathf.Clamp01(alpha);

            Color32 c = text.color.ToColor32();
            c.a = (byte)(alpha * 255);//更改alpha
            SetColor(text, c);
        }
        public static void SetAlpha(this TMP_Text text, float alpha, int startIndex, int endIndex)
        {
            alpha = Mathf.Clamp01(alpha);

            Color32 c = text.color.ToColor32();
            c.a = (byte)(alpha * 255);//更改alpha
            SetColor(text, c, startIndex, endIndex);
        }
        
        /// <summary>
        /// 根据传入Color32值更改
        /// </summary>
        public static void SetColor(this TMP_Text text, Color32 color)
        {
            for (int i = 0; i < text.textInfo.meshInfo[0].colors32.Length; i++)
            {
                text.textInfo.meshInfo[0].colors32[i] = color;
            }
            text.UpdateVertexData();//刷新
        }
        public static void SetColor(this TMP_Text text, Color32 color, int startIndex, int endIndex)
        {
            //0-0 1-4 2-8 3-12
            //0-3 1-7 2-11 3-15
            for (int i = startIndex * 4; i <= endIndex * 4 + 3; i++)
            {
                text.textInfo.meshInfo[0].colors32[i] = color;
            }
            text.UpdateVertexData();//刷新
        }
    
        public static void ApplyEffects(this MText text)
        {
            text.GetCurParsedText().ApplyEffects(text.mAnimator.mAnim);
        }

        public static void ApplyEffect(this MText text, int index)
        {
            ParsedText pText = text.GetCurParsedText();
            pText.ApplyEffect(text.mAnimator.mAnim, pText.animatorInfoList[index]);
        }

        public static void SetMText(this MText text, string content)
        {
            //由于SetAlpha()并不能即使生效，所以先隐藏，在调用之前显示即可
            text.alpha = 0;

            text.text = content;
            if (text.enableLocalization || text.enableTextAnimatorInline)
            {
                text.parsedTextDic = text.GenerateParsedTextDic();
                text.UpdateView();
            }
            text.mAnimator.PlayNewText();
        }
        /// <summary>
        /// 通过localID更新文字
        /// </summary>
        public static void SetMText(this MText text, int localID)
        {
            //Tip：由于使用本地化，所以需要将localID设置为非-1

            //由于SetAlpha()并不能即使生效，所以先隐藏，在调用之前显示即可
            text.alpha = 0;

            text.UpdateParsedTextDic(localID);
            text.mAnimator.PlayNewText();
        }
    }
}