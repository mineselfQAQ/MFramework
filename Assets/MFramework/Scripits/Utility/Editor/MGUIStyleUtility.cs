using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public static class MGUIStyleUtility
    {
        private static int ms_DefaultFontSize = 13;
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

        private static GUIStyle m_H1Style;
        public static GUIStyle H1Style
        {
            get
            {
                if (m_H1Style == null)
                {
                    GUIStyle style = new GUIStyle();

                    style.fontSize = 22;
                    style.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontStyle = FontStyle.Bold;

                    m_H1Style = style;
                }

                return m_H1Style;
            }
        }

        private static GUIStyle m_H2Style;
        public static GUIStyle H2Style
        {
            get
            {
                if (m_H2Style == null)
                {
                    GUIStyle style = new GUIStyle();

                    style.fontSize = 15;
                    style.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontStyle = FontStyle.Bold;

                    m_H2Style = style;
                }

                return m_H2Style;
            }
        }

        private static GUIStyle m_CenterStyle;
        public static GUIStyle CenterStyle
        {
            get
            {
                if (m_CenterStyle == null)
                {
                    GUIStyle style = new GUIStyle();
                    style.alignment = TextAnchor.MiddleCenter;

                    m_CenterStyle = style;
                }

                return m_CenterStyle;
            }
        }

        public static GUIStyle ColorStyle(Color color)
        {
            GUIStyle style = new GUIStyle();

            style.fontSize = ms_DefaultFontSize;
            style.normal.textColor = color;

            return style;
        }
    }
}
