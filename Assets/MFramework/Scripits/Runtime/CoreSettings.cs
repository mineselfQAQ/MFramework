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
            language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;//��ǰ��������
            SFXSound = 1;
            MusicSound = 1;
        }
    }
}
