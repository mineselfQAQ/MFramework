using System;
using System.IO;
using MFramework.Core;
using UnityEditor;
using UnityEngine;

namespace MFramework.Editor.UI
{
    internal static class UIEditorUtility
    {
        public static int DrawInt(SerializedProperty property, GUIContent content)
        {
            int value = EditorGUILayout.IntField(content, property.intValue);
            property.intValue = value;
            return value;
        }

        public static T DrawPopup<T>(SerializedProperty property, GUIContent content) where T : Enum
        {
            T value = (T)(object)property.enumValueIndex;
            value = (T)EditorGUILayout.EnumPopup(content, value);
            property.enumValueIndex = Convert.ToInt32(value);
            return value;
        }

        public static Texture GetIcon(Type type)
        {
            Texture systemIcon = EditorGUIUtility.ObjectContent(null, type).image;
            Texture scriptIcon = EditorGUIUtility.IconContent("cs Script Icon").image;
            return systemIcon != null ? systemIcon : scriptIcon;
        }

        public static string DisplayGenerateFileDialog(string path, string fileName = "")
        {
            int state = EditorUtility.DisplayDialogComplex(
                "Generating",
                $"Generate file at:\n{path}",
                "Confirm",
                "Cancel",
                "Change Path");

            if (state == 0) return path;
            if (state == 1)
            {
                MLog.Default?.W($"Canceled generating {fileName}.");
                return null;
            }

            string folder = EditorUtility.OpenFolderPanel("Select Path", Path.GetDirectoryName(Application.dataPath), string.Empty);
            return string.IsNullOrEmpty(folder) ? null : Path.Combine(folder, fileName);
        }
    }
}
