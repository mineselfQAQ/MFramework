using UnityEngine;

namespace MFramework
{
    public static class EditorSettingsBase
    {
        public enum PathName
        {
            ExcelGenerationPath,
            TableCSGenerationPath,
        }

        public static string defaultExcelGenerationPath = @$"{Application.dataPath}\..\ExcelData";
        public static string defaultTableCSGenerationPath = @$"{Application.dataPath}\TableCS";

        public static string GetPathName(PathName name)
        {
            switch (name)
            {
                case PathName.ExcelGenerationPath:
                    return "excelGenerationPath";
                case PathName.TableCSGenerationPath:
                    return "tableCSGenerationPath";
                default:
                    return null;
            }
        }
    }
}
