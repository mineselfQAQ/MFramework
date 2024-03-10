using MFramework;
using UnityEditor;

public static class MGUIUtility
{
    public static void DrawTitle(int spacing, string titleName)
    {
        EditorGUILayout.Space(spacing);
        EditorGUILayout.LabelField(titleName, MGUIStyleUtility.TitleStyle);
        EditorGUILayout.Space(spacing);
    }
}
