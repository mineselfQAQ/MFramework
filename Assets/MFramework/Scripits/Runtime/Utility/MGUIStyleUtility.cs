using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    public static class MGUIStyleUtility
    {
        private static Dictionary<int, GUIStyle> styleDic = new Dictionary<int, GUIStyle>();

        public static GUIStyle GetStyle(int fontSize, Color? color = null)
        {
            if(styleDic.ContainsKey(fontSize)) return styleDic[fontSize];

            Color c = color ?? Color.red;
            return CreateStyle(fontSize, c, FontStyle.Bold, TextAnchor.UpperLeft);
        }

        private static GUIStyle CreateStyle(int fontSize, Color color, FontStyle fontStyle, TextAnchor alignment)
        {
            GUIStyle style = new GUIStyle
            {
                fontSize = fontSize,
                normal = { textColor = color },
                fontStyle = fontStyle,
                alignment = alignment
            };

            return style;
        }
    }
}
