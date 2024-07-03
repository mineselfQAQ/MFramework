using MFramework;
using UnityEditor;
using UnityEngine;

public class WelcomePage : EditorWindow
{
    private Texture2D LOGOTex;
    private bool showState;

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

        bool state = EditorPrefs.GetBool(EditorPrefsData.WelcomePageState, true);
        showState = !state;
    }

    private void OnGUI()
    {
        GUILayout.Space(5);

        MGUIUtility.DrawTexture(LOGOTex, MGUIStyleUtility.CenterStyle);
        MGUIUtility.DrawH1("��ӭ");

        EditorGUILayout.LabelField("��ӭʹ��MFramework��Ŀǰ�ÿ��ֻ�����Ǹ������ܵļ��ϣ������");
        EditorGUILayout.LabelField("�����²��֣�");
        EditorGUILayout.LabelField("1.MFramework---���Ŀ��");
        EditorGUILayout.LabelField("2.MFramework_Demo---ʹ�ÿ�ܱ�д��Demo");
        EditorGUILayout.LabelField("3.MFramework_Test---����Լ�ĳЩ֪ʶ��ļ򵥲���");
        EditorGUILayout.LabelField("");
        EditorGUILayout.LabelField("��л���ʹ��~~~");

        GUILayout.FlexibleSpace();//����ײ����Ƶ�һ�ַ�ʽ

        DrawShowStateToggle();

        GUILayout.Space(8);
    }

    private void DrawShowStateToggle()
    {
        GUILayout.BeginHorizontal();
        {
            EditorGUIUtility.labelWidth = 55f;//���������밴ť֮��ľ���
            GUILayout.FlexibleSpace();//�Ҷ����һ�ַ�ʽ
            showState = EditorGUILayout.Toggle("������ʾ", showState);
            GUILayout.Space(5);//���Ҳ�����һ��ռ�

            if (showState) EditorPrefs.SetBool(EditorPrefsData.WelcomePageState, false);
            else EditorPrefs.SetBool(EditorPrefsData.WelcomePageState, true);
        }
        GUILayout.EndHorizontal();
    }
}
