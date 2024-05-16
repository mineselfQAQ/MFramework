using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MFramework
{
    public enum EditorPathName
    {
        ExcelGenerationPath,
        ExcelCSGenerationPath,
        ExcelBINGenerationPath
    }

    public static class EditorSettingsBase
    {
        //TODO:如果打包，EditorSettings很有可能无法获取
        //Tip：EditorSetting类并非MFramework框架下的内容，而是项目中的内容

        public static string defaultExcelGenerationPath;
        public static string defaultExcelCSGenerationPath;
        public static string defaultExcelBINGenerationPath;

        public static Dictionary<string, string> pathDic;//key---变量名  value---路径

        static EditorSettingsBase()
        {
            string rootPath = Path.GetDirectoryName(Application.dataPath);
            rootPath = rootPath.Replace("\\", "/");

            string streamingAssetsPath = Application.streamingAssetsPath.Replace("\\", "/");

            defaultExcelGenerationPath = @$"{rootPath}/ExcelData";
            defaultExcelCSGenerationPath = @$"{rootPath}/Assets/TableCS";
            defaultExcelBINGenerationPath = @$"{streamingAssetsPath}/ExcelBIN";

            pathDic = new Dictionary<string, string>()
            {
                { GetPathName(EditorPathName.ExcelGenerationPath), defaultExcelGenerationPath },
                { GetPathName(EditorPathName.ExcelCSGenerationPath), defaultExcelCSGenerationPath },
                { GetPathName(EditorPathName.ExcelBINGenerationPath), defaultExcelBINGenerationPath }
            };
        }

        public static string GetPathName(EditorPathName name)
        {
            switch (name)
            {
                case EditorPathName.ExcelGenerationPath:
                    return "excelGenerationPath";
                case EditorPathName.ExcelCSGenerationPath:
                    return "excelCSGenerationPath";
                case EditorPathName.ExcelBINGenerationPath:
                    return "excelBINGenerationPath";
                default:
                    return null;
            }
        }
    }
}
