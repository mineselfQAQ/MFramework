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
        //TODO:��������EditorSettings���п����޷���ȡ
        //Tip��EditorSetting�ಢ��MFramework����µ����ݣ�������Ŀ�е�����

        public static string defaultExcelGenerationPath;
        public static string defaultExcelCSGenerationPath;
        public static string defaultExcelBINGenerationPath;

        public static Dictionary<string, string> pathDic;//key---������  value---·��

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
