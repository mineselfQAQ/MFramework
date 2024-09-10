using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Conversation : ScriptableObject
{
#if UNITY_EDITOR
    [MenuItem("Assets/MCreate/3DGame/Conversation", false, priority = 3, secondaryPriority = 1.0f)]
    internal static void Create()
    {
        var asset = ScriptableObject.CreateInstance<Conversation>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (Path.GetExtension(path) != "")//选中的是文件
        {
            path = path.Replace(Path.GetFileName(path), "");
        }
        path = $"{path}/New{typeof(Conversation)}.asset";

        AssetDatabase.CreateAsset(asset, path);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
#endif

    public List<Sentence> sentences = new List<Sentence>();
}