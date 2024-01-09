using MFramework;
using UnityEditor;

public class DarkThemeSwitcher : EditorWindow
{
    [MenuItem("Tools/Toggle Dark Theme")] // 创建一个菜单项，用于切换深色主题
    public static void Toggle()
    {
        Log.Print(EditorGUIUtility.isProSkin);
    }
}