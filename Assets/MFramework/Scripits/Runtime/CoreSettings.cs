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
            SFXSound = 0.5f;
            MusicSound = 0.5f;
        }
    }
}
