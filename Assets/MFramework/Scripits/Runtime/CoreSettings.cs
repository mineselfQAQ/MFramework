using System;
using System.Globalization;

namespace MFramework
{
    [Serializable]
    public class CoreSettings
    {
        public string language;

        //����
        public int a;
        public float b;
        public string c;

        public CoreSettings()
        {
            language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;//��ǰ��������
        }
    }
}
