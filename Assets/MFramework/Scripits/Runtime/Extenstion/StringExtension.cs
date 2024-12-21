using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MFramework
{
    public static class StringExtension
    {
        public static string CD(this string str, int level = 1)
        {
            if (level <= 0) return str;

            string newStr = str;
            for (int i = 0; i < level; i++)
            {
                newStr = Path.GetDirectoryName(str);
            }
            return newStr.ReplaceSlash();
        }

        public static string ReplaceSlash(this string str, bool isForward = true)
        {
            if (isForward)
            {
                return str.Replace('\\', '/');
            }
            else
            {
                return str.Replace('/', '\\');
            }
        }

        public static string Bold(this string str)
        {
            return MLog.Bold(str);
        }

        public static string Italic(this string str)
        {
            return MLog.Italic(str);
        }

        public static string Color(this string str, Color color)
        {
            return MLog.Color(str, color);
        }
        public static string Color(this string str, Color color, bool isBold, bool isItalic)
        {
            return MLog.Color(str, color, isBold, isItalic);
        }
    }
}