using System.Linq;
using UnityEditor;
using UnityEngine;

public class RenameHierarchy : EditorWindow
{
    public string _NewName;
    public int _StartValue = 0;

    //操作---重命名
    //有：
    //NewName---更新后的名字
    //StartValue---后缀起始值
    [MenuItem("MFramework/HierarchyRename", false, 1001)]
    public static void Init()
    {
        RenameHierarchy window = GetWindow<RenameHierarchy>(true, "RenameTool");
        window.minSize = new Vector2(320, 150);
        window.maxSize = new Vector2(320, 1000);
        window.Show();
    }


    private void OnGUI()
    {
        //***设置NewName和StartValue***
        _NewName = EditorGUILayout.TextField("NewName:", _NewName);
        _StartValue = EditorGUILayout.IntField("StartValue", _StartValue);



        //===========================================================
        EditorGUILayout.Space(20);
        //===========================================================



        //***显示更改前与更改后名字的变化***
        var selectObject = Selection.gameObjects.OrderBy(obj => obj.transform.GetSiblingIndex());

        bool hasObject = Selection.objects.Length > 0;
        if (hasObject)
        {
            EditorGUILayout.LabelField("更改后:");
        }

        int i = 0;
        foreach (var obj in selectObject)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{obj.name}--->{_NewName}_{_StartValue + i}");
            EditorGUILayout.EndHorizontal();
            i++;
        }



        //===========================================================
        EditorGUILayout.Space(20);
        //===========================================================



        //***执行操作***
        //强调当前状态：
        //绿色---可以执行
        //红色---不能执行，因为没有选中物体，所以点下去也没反应
        GUI.enabled = hasObject;//激活状态，红色的时候是灰的---不可点击
        if (hasObject)
        {
            GUI.color = Color.green;
        }
        else
        {
            GUI.color = Color.red;
        }

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Execute"))//一般执行
            {
                i = 0;
                foreach (var obj in selectObject)
                {
                    obj.name = $"{_NewName}_{_StartValue + i}";
                    i++;
                }
            }
            if (GUILayout.Button("Execute(NoSuffix)"))//不带后缀执行
            {
                i = 0;
                foreach (var obj in selectObject)
                {
                    obj.name = $"{_NewName}";
                    i++;
                }
            }
            if (GUILayout.Button("Execute(OnlySuffix)"))//只输出后缀执行，如：0 1 2 3
            {
                i = 0;
                foreach (var obj in selectObject)
                {
                    obj.name = $"{_StartValue + i}";
                    i++;
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}