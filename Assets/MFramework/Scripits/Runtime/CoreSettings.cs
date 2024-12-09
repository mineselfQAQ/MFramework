using System;
using System.Globalization;

namespace MFramework
{
    [Serializable]
    public class CoreSettings
    {
        public string language;

        public CoreSettings()
        {
            language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;//当前地区语言
        }
    }
}
