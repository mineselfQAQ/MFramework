using MFramework;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GetProjectFilePath
{
    [MenuItem("Assets/Get File Path", priority = 2000)]
    public static void GetFilePath()
    {
        if (CheckAvailability())
        {
            Object obj = Selection.objects[0];
            string path = AssetDatabase.GetAssetPath(obj);
            GUIUtility.systemCopyBuffer = path;
            MLog.Print($"文件路径:{path}，已复制至黏贴板中");
        }
    }

    [MenuItem("Assets/Get Root File Path", priority = 2001)]
    public static void GetRootFilePath()
    {
        if (CheckAvailability())
        {
            Object obj = Selection.objects[0];
            string path = AssetDatabase.GetAssetPath(obj);
            path = Path.GetFullPath(path);
            path = path.Replace("\\", "/");
            GUIUtility.systemCopyBuffer = path;
            MLog.Print($"文件路径:{path}，已复制至黏贴板中");
        }
    }

    private static bool CheckAvailability()
    {
        var objs = Selection.objects;

        if (objs.Length != 1)
        {
            MLog.Print("请选择一个物体", MLogType.Warning);
            return false;
        }

        return true;
    }
}
