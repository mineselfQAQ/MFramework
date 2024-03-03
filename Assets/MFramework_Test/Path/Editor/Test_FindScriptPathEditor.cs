using MFramework;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Test_FindScriptPath))]
public class Test_FindScriptPathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("FindScriptPath"))
        {
            FindScriptPath();
        }

    }

    private void FindScriptPath()
    {
        string secondPath = GetSecondPath();

        if (secondPath != null)
        {
            string fullPath = Path.GetFullPath(secondPath);
            Log.Print(fullPath);
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
                Log.Print($"不是{Log.BoldWord("Prefab")}，无法获取", MLogType.Error);
                return null;
            }
        }

        //去除文件名
        return Path.GetDirectoryName(secondpath);
    }
}
