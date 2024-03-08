using UnityEngine;

namespace MFramework
{
    public static class EditorSettingsBase
    {
        public enum PathName
        {
            ExcelGenerationPath
        }

        public static string defaultExcelGenerationPath = @$"{Application.dataPath}\..\ExcelData";

        public static string GetPathName(PathName name)
        {
            switch (name)
            {
                case PathName.ExcelGenerationPath:
                    return "excelGenerationPath";
                default:
                    return null;
            }
        }
    }
}
