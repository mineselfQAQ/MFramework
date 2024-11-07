using MFramework;
using UnityEditor;
using UnityEngine;

public class WelcomePage : EditorWindow
{
    private Texture2D LOGOTex;

    [MenuItem("MFramework/WelcomePage", false, 1)]
    public static void Init()
    {
        WelcomePage window = GetWindow<WelcomePage>(true, "MFramework", false);
        window.minSize = new Vector2(425, 300);
        window.maxSize = new Vector2(425, 300);
        window.Show();
    }

    private void OnEnable()
    {
        LOGOTex = AssetDatabase.LoadAssetAtPath<Texture2D>(EditorResourcesPath.LOGOPath);
    }

    private void OnGUI()
    {
        GUILayout.Space(5);

        MEditorGUIUtility.DrawTexture(LOGOTex, MEditorGUIStyleUtility.CenterStyle);
        MEditorGUIUtility.DrawH1("欢迎");

        EditorGUILayout.LabelField("欢迎使用MFramework，目前该框架只能算是各个功能的集合，请见谅");
        EditorGUILayout.LabelField("有以下部分：");
        EditorGUILayout.LabelField("1.MFramework---核心框架");
        EditorGUILayout.LabelField("2.MFramework_Demo---使用框架编写的Demo");
        EditorGUILayout.LabelField("3.MFramework_Test---框架以及某些知识点的简单测试");
        EditorGUILayout.LabelField("");
        EditorGUILayout.LabelField("感谢你的使用~~~");

        GUILayout.Space(8);
    }
}
