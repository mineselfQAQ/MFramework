using MFramework;
using System.IO;
using UnityEditor;

public class InitializeScript
{
    [InitializeOnLoadMethod]
    public static void InitializeDefaultPath()
    {
        //问题：不应该是默认路径
        //if (!Directory.Exists(EditorSettingsBase.defaultExcelGenerationPath))
        //{
        //    Directory.CreateDirectory(EditorSettingsBase.defaultExcelGenerationPath);
        //    MLog.Print($"已初始化创建{EditorSettingsBase.GetPathName(EditorSettingsBase.PathName.ExcelGenerationPath)}  " +
        //        $"位置：{EditorSettingsBase.defaultExcelGenerationPath}");
        //}
    }
}
