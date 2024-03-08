using MFramework;
using System.IO;
using UnityEditor;

public class InitializeScript
{
    [InitializeOnLoadMethod]
    public static void InitializeDefaultExcelGenerationPath()
    {
        if (!Directory.Exists(EditorSettingsBase.defaultExcelGenerationPath))
        {
            Directory.CreateDirectory(EditorSettingsBase.defaultExcelGenerationPath);
            Log.Print($"已初始化创建{EditorSettingsBase.GetPathName(EditorSettingsBase.PathName.ExcelGenerationPath)}  " +
                $"位置：{EditorSettingsBase.defaultExcelGenerationPath}");
        }
    }
}
