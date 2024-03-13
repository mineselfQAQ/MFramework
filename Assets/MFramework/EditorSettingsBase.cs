using System.IO;
using UnityEngine;

namespace MFramework
{
    public static class EditorSettingsBase
    {
        public enum PathName
        {
            ExcelGenerationPath,
            ExcelCSGenerationPath,
            ExcelBINGenerationPath
        }

        public static string defaultExcelGenerationPath = 
            @$"{Path.GetDirectoryName(Application.dataPath)}\ExcelData".Replace("/","\\");

        public static string defaultExcelCSGenerationPath =
            @$"{Application.dataPath}\TableCS".Replace("/", "\\");

        public static string defaultExcelBINGenerationPath =
            @$"{Application.dataPath}\Resources\ExcelBIN".Replace("/", "\\");

        public static string GetPathName(PathName name)
        {
            switch (name)
            {
                case PathName.ExcelGenerationPath:
                    return "excelGenerationPath";
                case PathName.ExcelCSGenerationPath:
                    return "excelCSGenerationPath";
                case PathName.ExcelBINGenerationPath:
                    return "excelBINGenerationPath";
                default:
                    return null;
            }
        }
    }
}
