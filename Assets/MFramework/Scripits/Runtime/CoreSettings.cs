using System;
using System.Globalization;

namespace MFramework
{
    [Serializable]
    public class CoreSettings
    {
        public string language;

        public float SFXSound;
        public float MusicSound;

        public CoreSettings()
        {
            language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;//当前地区语言
            SFXSound = 0.5f;
            MusicSound = 0.5f;
        }
    }
}
