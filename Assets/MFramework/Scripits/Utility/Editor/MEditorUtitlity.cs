using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    internal static class MEditorUtitlity
    {
        internal static Texture GetIcon(Type type)
        {
            Texture systemIcon = EditorGUIUtility.ObjectContent(null, type).image;
            Texture customIcon = null;
            Texture csScriptIcon = EditorGUIUtility.IconContent("cs Script Icon").image;

            if (type == typeof(TMPro.TMP_InputField))
            {
                customIcon = (Texture2D)EditorGUIUtility.Load("Packages/com.unity.textmeshpro/Editor Resources/Gizmos/TMP - Input Field Icon.psd");
            }
            else if (type == typeof(TMPro.TMP_Dropdown))
            {
                customIcon = (Texture2D)EditorGUIUtility.Load("Packages/com.unity.textmeshpro/Editor Resources/Gizmos/TMP - Dropdown Icon.psd");
            }
            else if (type == typeof(TMPro.TextMeshProUGUI))
            {
                customIcon = (Texture2D)EditorGUIUtility.Load("Packages/com.unity.textmeshpro/Editor Resources/Gizmos/TMP - Text Component Icon.psd");
            }

            return systemIcon ?? customIcon ?? csScriptIcon;
        }

        internal static string ChangePath()
        {
            string guideFolder = Path.GetDirectoryName(Application.dataPath);
            string newPath = EditorUtility.OpenFolderPanel("ÇëÑ¡ÔñÂ·¾¶", guideFolder, "");

            if (newPath == "")
            {
                return null;
            }
            return newPath;
        }
    }
}