using MFramework;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(Test_FindPrefabPath))]
public class Test_FindPrefabPathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("FindPrefabPath"))
        {
            FindPrefabPath();
        }
    }

    private void FindPrefabPath()
    {
        string secondPath = GetSecondPath();

        if (secondPath != null)
        {
            string fullPath = Path.GetFullPath(secondPath);
            MLog.Print(fullPath);
            System.Diagnostics.Process.Start("explorer", "/select,\"" + fullPath + "\"");
        }
    }

    private string GetSecondPath()
    {
        //获取后半段路径
        string secondpath = AssetDatabase.GetAssetPath(target);
        if (secondpath == "")
        {
            secondpath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target);
        }
        if (secondpath == "")
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                secondpath = prefabStage.assetPath;
            }
            else
            {
                MLog.Print($"不是{MLog.BoldWord("Prefab")}，无法获取", MLogType.Error);
                return null;
            }
        }

        //去除文件名
        return Path.GetDirectoryName(secondpath);
    }
}
