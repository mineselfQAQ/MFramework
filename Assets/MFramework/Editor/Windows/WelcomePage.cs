using MFramework;
using UnityEditor;
using UnityEngine;

public class WelcomePage : EditorWindow
{
    private Texture2D LOGOTex;
    private bool showState;

    [MenuItem("MFramework/WelcomePage")]
    public static void Init()
    {
        WelcomePage window = GetWindow<WelcomePage>(true, "MFramework", false);
        window.minSize = new Vector2(300, 400);
        window.maxSize = new Vector2(300, 400);
        window.Show();
    }

    private void OnEnable()
    {
        LOGOTex = AssetDatabase.LoadAssetAtPath<Texture2D>(EditorResourcesPath.LOGOPath);

        bool state = EditorPrefs.GetBool(EditorPrefsData.WelcomePageState, true);
        showState = !state;
    }

    private void OnGUI()
    {
        GUILayout.Space(5);

        MGUIUtility.DrawTexture(LOGOTex, MGUIStyleUtility.CenterStyle);
        MGUIUtility.DrawH1("欢迎");

        EditorGUILayout.LabelField("欢迎使用MFramework，以下为一些基本用法：");
        EditorGUILayout.LabelField("1.xxxxxx");
        EditorGUILayout.LabelField("2.xxxxxx");
        EditorGUILayout.LabelField("3.xxxxxx");
        EditorGUILayout.LabelField("4.xxxxxx");
        EditorGUILayout.LabelField("5.xxxxxx");

        GUILayout.FlexibleSpace();//在最底部绘制的一种方式

        DrawShowStateToggle();

        GUILayout.Space(8);
    }

    private void DrawShowStateToggle()
    {
        GUILayout.BeginHorizontal();
        {
            EditorGUIUtility.labelWidth = 55f;//拉近文字与按钮之间的距离
            GUILayout.FlexibleSpace();//右对齐的一种方式
            showState = EditorGUILayout.Toggle("不再显示", showState);
            GUILayout.Space(5);//在右侧留出一点空间

            if (showState) EditorPrefs.SetBool(EditorPrefsData.WelcomePageState, false);
            else EditorPrefs.SetBool(EditorPrefsData.WelcomePageState, true);
        }
        GUILayout.EndHorizontal();
    }
}
