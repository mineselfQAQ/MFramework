using System.Collections.Generic;

namespace MFramework
{
    public enum MConfigurableName
    {
        ExcelGenerationPath,
        ExcelCSGenerationPath
    }

    public static class MConfigurableSettingsBase
    {
        public static string defaultExcelGenerationPath;
        public static string defaultExcelCSGenerationPath;

        public static Dictionary<string, string> pathDic;//key---变量名  value---路径

        static MConfigurableSettingsBase()
        {
            defaultExcelGenerationPath = @$"{MSettings.RootPath}/ExcelData";
            defaultExcelCSGenerationPath = @$"{MSettings.AssetPath}/TableCS";

            pathDic = new Dictionary<string, string>()
            {
                { GetPathName(MConfigurableName.ExcelGenerationPath), defaultExcelGenerationPath },
                { GetPathName(MConfigurableName.ExcelCSGenerationPath), defaultExcelCSGenerationPath }
            };
        }

        public static string GetPathName(MConfigurableName name)
        {
            switch (name)
            {
                case MConfigurableName.ExcelGenerationPath:
                    return "ExcelPath";
                case MConfigurableName.ExcelCSGenerationPath:
                    return "ExcelCSPath";
                default:
                    return null;
            }
        }
    }
}
