using System;
using System.Globalization;

namespace MFramework
{
    [Serializable]
    public class CoreSettings
    {
        public string language;

        //测试
        public int a;
        public float b;
        public string c;

        public CoreSettings()
        {
            language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;//当前地区语言
        }
    }
}
