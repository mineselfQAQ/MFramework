using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Windows;

namespace MFramework
{
    /// <summary>
    /// 逻辑：
    /// 类似格式化字符串，用{}代表可变文字
    /// </summary>
    public class MLocalizationString
    {
        public string originStr;
        public string formattedStr;

        public List<FormattedStr> formatStrList;

        public MLocalizationString(string originStr)
        {
            this.originStr = originStr;

            OriginStr2FormattedStr();
        }

        private void OriginStr2FormattedStr() 
        {
            formattedStr = DealStr(originStr);
            formatStrList = ExtractStr(originStr);
        }

        private string DealStr(string str)
        {
            string pattern = @"\{[^}]*\}";//{任意字符}
            int count = 0;
            string res = Regex.Replace(str, pattern, m => $"{{{count++}}}");//将所有{任意字符}替换为{当前计数}

            return res;
        }
        private List<FormattedStr> ExtractStr(string str)
        {
            string pattern = @"\{([^}]*)\}";//{任意字符}，任意字符被标记为组
            MatchCollection matches = Regex.Matches(str, pattern);
            List<FormattedStr> res = new List<FormattedStr>();
            foreach (Match match in matches)
            {
                string innerStr = match.Groups[1].Value;
                res.Add(new FormattedStr(innerStr));
            }

            return res;
        }

        public class FormattedStr
        {
            private int curState;//当前选项
            private string[] texts;//选项的具体文字

            public FormattedStr(string str)
            {
                string[] strs = str.Split('|');

                for(int i = 0; i < strs.Length; i++)
                {
                    if (strs[i].StartsWith('#'))
                    {
                        strs[i] = strs[i].Substring(1);
                        curState = i;//默认选项
                    }
                }

                texts = strs;
            }
        }
    }
}