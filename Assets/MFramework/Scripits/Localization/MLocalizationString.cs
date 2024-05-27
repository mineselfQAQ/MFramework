using System.Collections.Generic;
using System.Text.RegularExpressions;

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

        public bool isFormatted;

        public SupportLanguage language;

        public List<FormattedStr> formatStrList;

        public MLocalizationString(string originStr, SupportLanguage language)
        {
            this.originStr = originStr;
            this.language = language;

            OriginStr2FormattedStr();
        }

        public void ChangeState(int pos, int state)
        {
            if (pos < 0 || pos >= formatStrList.Count) return;

            formatStrList[pos].str = formatStrList[pos].texts[state];
            formatStrList[pos].curState = state;
        }

        private void OriginStr2FormattedStr() 
        {
            formattedStr = DealStr(originStr);
            if (formattedStr != originStr)
            {
                isFormatted = true;
                formatStrList = ExtractStr(originStr);
            }
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

        public override string ToString()
        {
            if (!isFormatted)//正常格式
            {
                return originStr;
            }
            else
            {
                string str = formattedStr;
                for (int i = 0; i < formatStrList.Count; i++)
                {
                    var cur = formatStrList[i];
                    str = str.Replace($"{{{i}}}", $"{cur}");
                }
                return str;
            }
        }

        public class FormattedStr
        {
            internal string str;

            internal int curState = 0;//当前选项，默认使用第一个选项
            internal string[] texts;//选项的具体文字

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
                this.str = texts[curState];
            }

            public override string ToString()
            {
                return str;
            }
        }
    }
}