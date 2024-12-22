using System;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public static class MEditorGUIUtility
    {
        public static void DrawH1(string titleName)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(titleName, MEditorGUIStyleUtility.H1Style);
            EditorGUILayout.Space(5);
        }

        public static void DrawH2(string titleName)
        {
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField(titleName, MEditorGUIStyleUtility.H2Style);
            EditorGUILayout.Space(2);
        }

        public static void DrawLeftH2(string titleName)
        {
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField(titleName, MEditorGUIStyleUtility.LeftH2Style);
            EditorGUILayout.Space(2);
        }

        public static void DrawH3(string titleName)
        {
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField(titleName, MEditorGUIStyleUtility.H3Style);
        }

        public static void DrawLeftH3(string titleName)
        {
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField(titleName, MEditorGUIStyleUtility.LeftH3Style);
        }

        public static void DrawText(string message, string tip, params GUILayoutOption[] content)
        {
            GUIContent GUIContent = new GUIContent(message, tip);
            EditorGUILayout.LabelField(GUIContent, content);
        }
        public static void DrawText(string message, params GUILayoutOption[] content)
        {
            EditorGUILayout.LabelField(message, content);
        }
        public static void DrawText(string message, MGUIContext context = MGUIContext.None)
        {
            switch (context)
            {
                case MGUIContext.Bold:
                    EditorGUILayout.LabelField(message, MEditorGUIStyleUtility.BoldStyle);
                    break;
                case MGUIContext.RedColor:
                    EditorGUILayout.LabelField(message, MEditorGUIStyleUtility.ColorStyle(Color.red));
                    break;
                case MGUIContext.None:
                    EditorGUILayout.LabelField(message);
                    break;
            }
        }

        public static void DrawTexture(Texture2D tex, GUIStyle style)
        {
            if(tex != null) 
            {
                GUILayout.Label(tex, style);
            }
        }

        public static void Horizontal(Action action)
        {
            EditorGUILayout.BeginHorizontal();
            action?.Invoke();
            EditorGUILayout.EndHorizontal();
        }
        public static void Vertical(Action action)
        {
            EditorGUILayout.BeginVertical();
            action?.Invoke();
            EditorGUILayout.EndVertical();
        }
    }

    public enum MGUIContext
    {
        Bold,
        RedColor,

        None
    }
}