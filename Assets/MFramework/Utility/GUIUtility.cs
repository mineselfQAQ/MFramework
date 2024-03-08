using MFramework;
using UnityEditor;

public static class GUIUtility
{
    public static void DrawTitle(int spacing, string titleName)
    {
        EditorGUILayout.Space(spacing);
        EditorGUILayout.LabelField(titleName, GUIStyleUtility.TitleStyle);
        EditorGUILayout.Space(spacing);
    }
}
