using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    public static class StringExtension
    {
        public static string Bold(this string str)
        {
            return MLog.BoldWord(str);
        }

        public static string Italic(this string str)
        {
            return MLog.ItalicWord(str);
        }

        public static string Color(this string str, Color color)
        {
            return MLog.ColorWord(str, color);
        }
        public static string Color(this string str, Color color, bool isBold, bool isItalic)
        {
            return MLog.ColorWord(str, color, isBold, isItalic);
        }
    }
}