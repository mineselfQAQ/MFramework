using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public static class MGUIUtility
    {
        public static void DrawH1(string titleName)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(titleName, MGUIStyleUtility.H1Style);
            EditorGUILayout.Space(5);
        }

        public static void DrawH2(string titleName)
        {
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField(titleName, MGUIStyleUtility.H2Style);
            EditorGUILayout.Space(2);
        }

        public static void DrawLeftH2(string titleName)
        {
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField(titleName, MGUIStyleUtility.LeftH2Style);
            EditorGUILayout.Space(2);
        }

        public static void DrawTexture(Texture2D tex, GUIStyle style)
        {
            if(tex != null) 
            {
                GUILayout.Label(tex, style);
            }
        }
    }
}