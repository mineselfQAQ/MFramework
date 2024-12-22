using MFramework;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RenameTool : EditorWindow
{
    public string _newName;
    public int _startValue = 0;

    private Vector2 namePos;

    //操作---重命名
    //有：
    //NewName---更新后的名字
    //StartValue---后缀起始值
    [MenuItem("MFramework/Rename Tool", false, 902)]
    public static void Init()
    {
        RenameTool window = GetWindow<RenameTool>(true, "RenameTool");
        window.minSize = new Vector2(320, 150);
        window.maxSize = new Vector2(320, 1000);
        window.Show();
    }


    private void OnGUI()
    {
        //***设置NewName和StartValue***
        _newName = EditorGUILayout.TextField("NewName:", _newName);
        _startValue = EditorGUILayout.IntField("StartValue", _startValue);

        EditorGUILayout.Space(20);

        //***显示更改前与更改后名字的变化***
        var hierarchyObjs = Selection.gameObjects;
        var projectObjs = MSelection.projectObjects;
        bool selectedHierarchy = false, selectedProject = false;
        bool hasObject = false;

        if (hierarchyObjs.Length != 0) { selectedHierarchy = true; hasObject = true; }
        if (projectObjs.Length != 0) { selectedProject = true; hasObject = true; }

        if (selectedHierarchy && selectedProject)
        {
            MLog.Print($"{typeof(RenameTool)}：请勿同时选择Hierarchy和Project中的物体", MLogType.Warning);
            return;
        }

        Object[] objs = null;
        if (selectedHierarchy)
        {
            //TODO：这样应该会持续计算消耗导致鼠标进入加载状态，需要缓存
            var selectObject = Selection.gameObjects.OrderBy(obj => obj.transform.GetSiblingIndex());
            objs = selectObject.ToArray();
        }
        else
        {
            objs = projectObjs;
        }

        int i = 0;
        if (hasObject)
        {
            EditorGUILayout.LabelField("更改后:");

            namePos = EditorGUILayout.BeginScrollView(namePos);
            {
                foreach (var obj in objs)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{obj.name}--->{_newName}_{_startValue + i}");
                    EditorGUILayout.EndHorizontal();
                    i++;
                }
            }
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.Space(20);

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
                foreach (var obj in objs)
                {
                    if (selectedHierarchy)
                    {
                        obj.name = $"{_newName}_{_startValue + i}";
                        EditorUtility.SetDirty(obj);
                    }
                    else
                    {
                        string path = AssetDatabase.GetAssetPath(obj);
                        AssetDatabase.RenameAsset(path, $"{_newName}_{_startValue + i}");
                    }
                    i++;
                }
            }
            if (GUILayout.Button("Execute(NoSuffix)"))//不带后缀执行
            {
                i = 0;
                foreach (var obj in objs)
                {
                    if (selectedHierarchy)
                    {
                        obj.name = $"{_newName}";
                        EditorUtility.SetDirty(obj);
                    }
                    else
                    {
                        string path = AssetDatabase.GetAssetPath(obj);
                        AssetDatabase.RenameAsset(path, $"{_newName}");
                    }
                    i++;
                }
            }
            if (GUILayout.Button("Execute(OnlySuffix)"))//只输出后缀执行，如：0 1 2 3
            {
                i = 0;
                foreach (var obj in objs)
                {
                    if (selectedHierarchy)
                    {
                        obj.name = $"{_startValue + i}";
                        EditorUtility.SetDirty(obj);
                    }
                    else
                    {
                        string path = AssetDatabase.GetAssetPath(obj);
                        AssetDatabase.RenameAsset(path, $"{_startValue + i}");
                    }
                    i++;
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        if (selectedProject)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}