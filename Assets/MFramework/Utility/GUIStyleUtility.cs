using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public static class GUIStyleUtility
    {
        private static int ms_DefaultFontSize = 14;
        private static Color ms_DefaultColor = EditorGUIUtility.isProSkin ? new Color(0.73f, 0.73f, 0.73f) : Color.black;

        private static GUIStyle m_defaultStyle;
        public static GUIStyle DefaultStyle
        {
            get
            {
                if (m_defaultStyle == null)
                {
                    GUIStyle style = new GUIStyle();

                    style.fontSize = ms_DefaultFontSize;
                    style.normal.textColor = ms_DefaultColor;

                    m_defaultStyle = style;
                }

                return m_defaultStyle;
            }
        }

        private static GUIStyle m_boldStyle;
        public static GUIStyle BoldStyle
        {
            get
            {
                if (m_boldStyle == null)
                {
                    GUIStyle style = new GUIStyle();

                    style.fontSize = ms_DefaultFontSize;
                    style.normal.textColor = ms_DefaultColor;
                    style.fontStyle = FontStyle.Bold;

                    m_boldStyle = style;
                }

                return m_boldStyle;
            }
        }

        private static GUIStyle m_TitleStyle;
        public static GUIStyle TitleStyle
        {
            get
            {
                if (m_TitleStyle == null)
                {
                    GUIStyle style = new GUIStyle();

                    style.fontSize = 20;
                    style.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontStyle = FontStyle.Bold;

                    m_TitleStyle = style;
                }

                return m_TitleStyle;
            }
        }
    }
}
