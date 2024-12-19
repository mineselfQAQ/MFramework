using MFramework.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MFramework
{
    public class ParsedText
    {
        private string originText;//原数据---不会改变
        private string localText;//处理了本地化后的字符串---不会改变
        private string localedText;//将本地化应用，但是没有处理动画的字符串---state更换后会改变
        private string parsedText;//当前的解析字符串(如果有本地化就会存在替换)---与localedText相关

        private SupportLanguage language;

        internal bool haveLocalInline;//开启本地化内联函数，如：{#他|她|它}在吗
        internal bool haveAnimatorInline;//开启动画内联函数，如：<wave>起伏的字</>

        private List<LocalInfo> localInfoList;//存储更改格式后{index}其中的信息
        internal List<AnimatorInfo> animatorInfoList;//动画信息

        /// <summary>
        /// 核心：分为4个阶段的数据
        /// 1.originText---直接收集到的原始数据，此时本地化内联{}与动画内联<></>没有去除
        /// 2.localText---进行本地化处理的数据，此时会将{文字}转换为{0}{1}的形式
        /// 3.localedText---应用当前本地化情况的数据，每个空({0})都会有多个选项，此时会选择具体的选项
        /// 4.parsedText---再应用动画的数据，此时即为最终数据
        /// </summary>
        public ParsedText(string text, SupportLanguage language)
        {
            originText = text;
            this.language = language;

            ParseText();
        }
        private void ParseText()
        {
            localText = GetLocalText(originText);
            if (localText != originText)//具有本地化内联
            {
                haveLocalInline = true;
                localInfoList = GetLocalInfo(originText);
                localedText = GetLocaledText(localText);

                DealAnimator(localedText);
            }
            else//不具有本地化内联
            {
                haveLocalInline = false;
                localText = null;

                DealAnimator(originText);
            }
        }

        public override string ToString()
        {
            if (!haveLocalInline && !haveAnimatorInline) return originText;

            return parsedText;
        }

        #region 公共方法
        /// <summary>
        /// 更新数据(本地化内联更换选项)
        /// </summary>
        public void UpdateData()
        {
            if (!haveLocalInline) return;//没有本地化内联，就没有需要更新的数据

            //大概率是由于使用了ChangeLocalState()更改了某个{}中的文字导致整体的变化
            localedText = GetLocaledText(localText);//更新localedText
            DealAnimator(localedText);//同步处理parsedText
        }

        /// <summary>
        /// 更改本地化内联数据
        /// </summary>
        internal void ChangeLocalState(int pos, int state)
        {
            if (pos < 0 || pos >= localInfoList.Count) return;

            localInfoList[pos].curStr = localInfoList[pos].texts[state];
            localInfoList[pos].curState = state;
        }

        internal int GetCurState(int pos)
        {
            if (pos < 0 || pos >= localInfoList.Count) return -1;

            return localInfoList[pos].curState;
        }
        #endregion

        #region 处理文本信息
        /// <summary>
        /// 处理本地化(将{xx|xx}更改为{index})
        /// </summary>
        private string GetLocalText(string originStr)
        {
            string pattern = @"\{[^}]*\}";//{任意字符}
            int count = 0;
            string res = Regex.Replace(originStr, pattern, m => $"{{{count++}}}");//将所有{任意字符}替换为{当前计数}

            return res;
        }
        private List<LocalInfo> GetLocalInfo(string originStr)
        {
            string pattern = @"\{([^}]*)\}";//{任意字符}，任意字符被标记为组
            MatchCollection matches = Regex.Matches(originStr, pattern);
            List<LocalInfo> res = new List<LocalInfo>();
            foreach (Match match in matches)
            {
                string innerStr = match.Groups[1].Value;
                res.Add(new LocalInfo(innerStr));
            }

            return res;
        }
        private string GetLocaledText(string localText)
        {
            if (string.IsNullOrEmpty(localText)) return null;

            localedText = localText;
            for (int i = 0; i < localInfoList.Count; i++)
            {
                localedText = localedText.Replace($"{{{i}}}", localInfoList[i].ToString());
            }
            return localedText;
        }
        private void DealAnimator(string str)
        {
            string pattern = @"<([^>]+)>(.*?)</>";

            //如：<wave(1,2)>ABC</>
            MatchCollection matches = Regex.Matches(str, pattern);

            if (matches.Count > 0)
            {
                haveAnimatorInline = true;
                animatorInfoList = new List<AnimatorInfo>();
            }
            else 
            {
                haveAnimatorInline = false;
                parsedText = str;
                return;
            } 

            foreach (Match match in matches)
            {
                string content = match.Groups[2].Value;//即ABC
                //match.Index---整体首索引(指在前<>前面的元素个数)
                //match.Value.IndexOf(content)---指前<>中的元素个数
                //before---直到<wave>前的内容(包括<wave>)
                int beforeLength = match.Index + match.Value.IndexOf(content);
                string beforeStr = str.Substring(0, beforeLength);
                //减去所有<wave>之前的tags(包括<wave>，不包括之后的一个</>)
                int startIndex = beforeLength - CountTagsLength(beforeStr) - CountSpacesLength(beforeStr);
                int endIndex = startIndex + content.Length - 1 - CountSpacesLength(content);
                string funcStr = match.Groups[1].Value;//即wave(1,2)
                string typeStr, argStr;
                GetTypeStrAndArgStr(funcStr, out typeStr, out argStr);
                animatorInfoList.Add(new AnimatorInfo(content, startIndex, endIndex, typeStr, argStr));
            }

            //去除<>(取中间)得到最终字符串
            parsedText = Regex.Replace(str, pattern, m => m.Groups[2].Value);
        }


        /// <summary>
        /// 计算所有<>的长度
        /// </summary>
        /// <param playerName="str"></param>
        /// <returns></returns>
        private int CountTagsLength(string str)
        {
            int length = 0;
            string pattern = @"<[^>]+>";

            MatchCollection matches = Regex.Matches(str, pattern);
            foreach (Match match in matches)
            {
                length += match.Length;
            }

            return length;
        }
        private int CountSpacesLength(string str)
        {
            int spaceCount = 0;
            foreach (char c in str)
            {
                if (char.IsWhiteSpace(c))
                {
                    spaceCount++;
                }
            }
            return spaceCount;
        }
        private void GetTypeStrAndArgStr(string funcStr, out string typeStr, out string argStr)
        {
            int n = funcStr.Length;
            int splitIndex = funcStr.IndexOf('(');
            if (splitIndex == -1)//没有添加形参情况
            {
                typeStr = funcStr;
                argStr = null;
                return;
            }
            typeStr = funcStr.Substring(0, splitIndex);
            argStr = funcStr.Substring(splitIndex + 1, n - splitIndex - 2);
        }
        #endregion

        /// <summary>
        /// 应用内联动画效果
        /// </summary>
        internal void ApplyEffects(MTextAnimation anim)
        {
            if (animatorInfoList == null) return;

            foreach (var info in animatorInfoList)
            {
                ApplyEffect(anim, info);
            }
        }
        internal void ApplyEffect(MTextAnimation anim, AnimatorInfo info)
        {
            if (info == null) return;

            int count = info.GetCount();//形参数量

            //颜色Color
            //color(speed,r,g,b)---速度默认为1
            //color(r,g,b)
            if (info.type == AnimatorType.Color)
            {
                if (!CheckArgsNum(count, 3, 4)) return;

                if (count == 3)
                {
                    anim.Color(1, new Color32((byte)info.args[0], (byte)info.args[1], (byte)info.args[2], 255), info.start, info.end);
                }
                else if (count == 4)
                {
                    anim.Color(info.args[0], new Color32((byte)info.args[1], (byte)info.args[2], (byte)info.args[3], 255), info.start, info.end);
                }
            }
            //缩放Scale
            //scale(scaleFactor, speed)---速度默认为1
            //scale(scaleFactor)
            else if (info.type == AnimatorType.Scale)
            {
                if (!CheckArgsNum(count, 1, 2)) return;

                if (count == 1)
                {
                    anim.Scale(1, info.args[0], info.start, info.end);
                }
                else if (count == 2)
                {
                    anim.Scale(info.args[1], info.args[0], info.start, info.end);
                }
            }
            //旋转Rotate  注：degree>0为顺时针
            //rotate(degree, speed)---速度默认为1
            //rotate(degree)
            else if (info.type == AnimatorType.Rotate)
            {
                if (!CheckArgsNum(count, 1, 2)) return;

                if (count == 1)
                {
                    anim.Rotate(1, info.args[0], info.start, info.end);
                }
                else if (count == 2)
                {
                    anim.Rotate(info.args[1], info.args[0], info.start, info.end);
                }
            }
            //渐入FadeIn
            //fadein(speed)---速度默认为1
            //fadein()
            else if (info.type == AnimatorType.FadeIn)
            {
                if (!CheckArgsNum(count, 0, 1)) return;

                if (count == 0)
                {
                    anim.FadeIn(1, info.start, info.end);
                }
                else if (count == 1)
                {
                    anim.FadeIn(1 * info.args[0], info.start, info.end);
                }
            }
            //渐出FadeOut
            //fadeout(speed)---速度默认为1
            //fadeout()
            else if (info.type == AnimatorType.FadeOut)
            {
                if (!CheckArgsNum(count, 0, 1)) return;

                if (count == 0)
                {
                    anim.FadeOut(1, info.start, info.end);
                }
                else if (count == 1)
                {
                    anim.FadeOut(1 * info.args[0], info.start, info.end);
                }
            }
            //循环渐变颜色ColorFade
            //colorfade(speed,r,g,b)---速度默认为1
            //colorfade(r,g,b)
            else if (info.type == AnimatorType.ColorFade)
            {
                if (!CheckArgsNum(count, 3, 4)) return;

                else if (count == 3)
                {
                    anim.ColorFade(1, new Color32((byte)info.args[0], (byte)info.args[1], (byte)info.args[2], 255), info.start, info.end);
                }
                else if (count == 4)
                {
                    anim.ColorFade(info.args[0], new Color32((byte)info.args[1], (byte)info.args[2], (byte)info.args[3], 255), info.start, info.end);
                }
            }
            //震动Shake
            //计时
            //shake(duration,amplitude,frequency) amplitude---强度(默认1) frequency---频率(默认1)
            //shake(duration)
            //无限
            //shake(amplitude,frequency)
            //shake()
            else if (info.type == AnimatorType.Shake)
            {
                if (!CheckArgsNum(count, 0, 1, 2, 3)) return;

                if (count == 0)
                {
                    anim.Shake(info.start, info.end);
                }
                else if (count == 1)
                {
                    anim.Shake(info.start, info.end, info.args[0]);
                }
                else if (count == 2)
                {
                    anim.Shake(info.start, info.end, -1, info.args[0], info.args[1]);
                }
                else if (count == 3)
                {
                    anim.Shake(info.start, info.end, info.args[0], info.args[1], info.args[2]);
                }
            }
            //倾斜Slant
            //slant(speed, degree)---速度默认为1 角度默认为15°
            //slant(speed)
            //slant()
            else if (info.type == AnimatorType.Slant)
            {
                if (!CheckArgsNum(count, 0, 1, 2)) return;

                if (count == 0)
                {
                    anim.Slant(1, info.start, info.end);
                }
                else if (count == 1)
                {
                    anim.Slant(info.args[0], info.start, info.end);
                }
                else if (count == 2)
                {
                    anim.Slant(info.args[0], info.start, info.end, info.args[1]);
                }
            }
            //闪烁Twinkle
            //twinkle(speed)
            //twinkle()---速度默认为1
            else if (info.type == AnimatorType.Twinkle)
            {
                if (!CheckArgsNum(count, 0, 1)) return;

                if (count == 0)
                {
                    anim.Twinkle(1, info.start, info.end);
                }
                else if (count == 1)
                {
                    anim.Twinkle(info.args[0], info.start, info.end);
                }
            }
            //尖叫Exclaim(震动+加粗)
            //计时
            //exclaim(speed,duration,scaleFactor,r,g,b)
            //exclaim(speed,duration)
            //无限
            //exclaim(speed,sacleFactor,r,g,b)
            //exclaim(speed)
            //exclaim()
            else if (info.type == AnimatorType.Exclaim)
            {
                if (!CheckArgsNum(count, 0, 1, 2, 5, 6)) return;

                if (count == 0)
                {
                    anim.Exclaim(1, info.start, info.end);
                }
                else if (count == 1)
                {
                    anim.Exclaim(info.args[0], info.start, info.end);
                }
                else if (count == 2)
                {
                    anim.ExclaimTimed(info.args[0], info.args[1], info.start, info.end);
                }
                else if (count == 5)
                {
                    anim.ExclaimColor(info.args[0], new Color32((byte)info.args[2], (byte)info.args[3], (byte)info.args[4], 255), info.start, info.end, info.args[1]);
                }
                else if (count == 6)
                {
                    anim.ExclaimColorTimed(info.args[0], info.args[1], new Color32((byte)info.args[3], (byte)info.args[4], (byte)info.args[5], 255), info.start, info.end, info.args[2]);
                }
            }
            //无限波动WaveLoop
            //wave(speed, amplitude, loopIntervalFactor) amplitude---强度(默认1) loopIntervalFactor---每次循环间隔时间(默认为1，大致为一轮结束直接开始下一轮)
            //wave(speed)
            //wave()
            else if (info.type == AnimatorType.WaveLoop)
            {
                if (!CheckArgsNum(count, 0, 1)) return;

                if (count == 0)
                {
                    anim.Wave(1, info.start, info.end);
                }
                else if (count == 1)
                {
                    anim.Wave(info.args[0], info.start, info.end);
                }
                else if (count == 3)
                {
                    anim.Wave(info.args[0], info.start, info.end, info.args[1], info.args[2]);
                }
            }
        }
        private bool CheckArgsNum(int count, params int[] validNums)
        {
            if (!validNums.Contains(count))
            {
                MLog.Print($"{typeof(ParsedText)}：{ToString()}内联函数形参数量不符合要求，请检查", MLogType.Warning);
                return false;
            }
            return true;
        }
    }

    public class LocalInfo
    {
        internal string curStr;

        internal int curState = 0;//当前选项，默认使用第一个选项
        internal string[] texts;//选项的具体文字

        public LocalInfo(string str)
        {
            string[] strs = str.Split('|');

            for (int i = 0; i < strs.Length; i++)
            {
                if (strs[i].StartsWith('#'))
                {
                    strs[i] = strs[i].Substring(1);
                    curState = i;//默认选项
                }
            }

            texts = strs;
            curStr = texts[curState];
        }

        public override string ToString()
        {
            return curStr;
        }
    }

    public class AnimatorInfo
    {
        internal string str;

        internal int start;
        internal int end;

        internal AnimatorType type;//动画类型
        internal List<float> args;//参数列表

        public AnimatorInfo(string str, int start, int end, string typeStr, string argStr)
        {
            this.str = str;
            this.start = start;
            this.end = end;

            GetType(typeStr);
            GetArgs(argStr);
        }

        public int GetCount()
        {
            if (args == null) return 0;

            return args.Count;
        }

        private void GetType(string typeStr)
        {
            typeStr = typeStr.ToLower();

            if (typeStr == MTextAnimatorName.COLORFADE)
            {
                type = AnimatorType.ColorFade;
            }
            else if (typeStr == MTextAnimatorName.COLOR)
            {
                type = AnimatorType.Color;
            }
            else if (typeStr == MTextAnimatorName.SLANT)
            {
                type = AnimatorType.Slant;
            }
            else if (typeStr == MTextAnimatorName.EXCLAIM)
            {
                type = AnimatorType.Exclaim;
            }
            else if (typeStr == MTextAnimatorName.FADEIN)
            {
                type = AnimatorType.FadeIn;
            }
            else if (typeStr == MTextAnimatorName.FADEOUT)
            {
                type = AnimatorType.FadeOut;
            }
            else if (typeStr == MTextAnimatorName.WAVELOOP)
            {
                type = AnimatorType.WaveLoop;
            }
            else if (typeStr == MTextAnimatorName.ROTATE)
            {
                type = AnimatorType.Rotate;
            }
            else if (typeStr == MTextAnimatorName.SCALE)
            {
                type = AnimatorType.Scale;
            }
            else if (typeStr == MTextAnimatorName.SHAKE)
            {
                type = AnimatorType.Shake;
            }
            else if (typeStr == MTextAnimatorName.TWINKLE)
            {
                type = AnimatorType.Twinkle;
            }
            else
            {
                MLog.Print($"{typeof(MText)}：未知的内联函数{typeStr}，请检查", MLogType.Warning);
                type = AnimatorType.None;
            }
        }

        private void GetArgs(string argStr)
        {
            if (string.IsNullOrEmpty(argStr)) return;

            args = new List<float>();

            string[] strs = argStr.Split(',');
            strs = strs.Select(s => s.Trim()).ToArray();//去除多余空格

            foreach (var str in strs)
            {
                args.Add(float.Parse(str));
            }
        }
    }

    public enum AnimatorType
    {
        None,

        ColorFade,
        FadeIn,
        FadeOut,
        Slant,
        Shake,
        Exclaim,
        WaveLoop,
        Scale,
        Rotate,
        Color,
        Twinkle
    }
}