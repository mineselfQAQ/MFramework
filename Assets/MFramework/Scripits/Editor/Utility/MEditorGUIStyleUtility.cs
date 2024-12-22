using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public static class MEditorGUIStyleUtility
    {
        private static int ms_DefaultFontSize = 13;
        private static Color ms_DefaultColor = GetColor(new Color(0.73f, 0.73f, 0.73f), Color.black);

        //»º´æ
        private static GUIStyle m_defaultStyle;
        private static GUIStyle m_boldStyle;
        private static GUIStyle m_H1Style;
        private static GUIStyle m_H2Style;
        private static GUIStyle m_LeftH2Style;
        private static GUIStyle m_H3Style;
        private static GUIStyle m_LeftH3Style;
        private static GUIStyle m_CenterStyle;

        public static GUIStyle DefaultStyle => 
            GetOrCreateStyle(m_defaultStyle, ms_DefaultFontSize, ms_DefaultColor, FontStyle.Normal, TextAnchor.UpperLeft);
        public static GUIStyle BoldStyle => 
            GetOrCreateStyle(m_boldStyle, ms_DefaultFontSize, ms_DefaultColor, FontStyle.Bold, TextAnchor.UpperLeft);
        public static GUIStyle H1Style => 
            GetOrCreateStyle(m_H1Style, 22, GetColor(Color.white, Color.black), FontStyle.Bold, TextAnchor.MiddleCenter);
        public static GUIStyle H2Style => 
            GetOrCreateStyle(m_H2Style, 15, GetColor(Color.white, Color.black), FontStyle.Bold, TextAnchor.MiddleCenter);
        public static GUIStyle LeftH2Style => 
            GetOrCreateStyle(m_LeftH2Style, 15, GetColor(Color.white, Color.black), FontStyle.Bold, TextAnchor.MiddleLeft);
        public static GUIStyle H3Style =>
            GetOrCreateStyle(m_H3Style, 12, GetColor(Color.white, Color.black), FontStyle.Bold, TextAnchor.MiddleCenter);
        public static GUIStyle LeftH3Style =>
            GetOrCreateStyle(m_LeftH3Style, 12, GetColor(Color.white, Color.black), FontStyle.Bold, TextAnchor.MiddleLeft);
        public static GUIStyle CenterStyle => 
            GetOrCreateStyle(m_CenterStyle, ms_DefaultFontSize, ms_DefaultColor, FontStyle.Normal, TextAnchor.MiddleCenter);

        private static GUIStyle GetOrCreateStyle(GUIStyle cachedStyle, int fontSize, Color color, FontStyle fontStyle, TextAnchor alignment)
        {
            if (cachedStyle == null)
            {
                GUIStyle style = new GUIStyle
                {
                    fontSize = fontSize,
                    normal = { textColor = color },
                    fontStyle = fontStyle,
                    alignment = alignment
                };
                cachedStyle = style;
            }

            return cachedStyle;
        }

        public static GUIStyle ColorStyle(Color color)
        {
            GUIStyle style = new GUIStyle();

            style.fontSize = ms_DefaultFontSize;
            style.normal.textColor = color;

            return style;
        }

        private static Color GetColor(Color brightColor, Color darkColor)
        {
            return EditorGUIUtility.isProSkin ? brightColor : darkColor;
        }
    }
}
