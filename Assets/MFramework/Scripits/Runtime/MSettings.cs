using System;
using UnityEngine;

namespace MFramework
{
    public class MSettings
    {
        public const string UICanvasName = "UICanvas";
        public const string UICameraName = "UICamera";

        public const string ResourceAssetName = "Assets/MTemp/AB/Resource.bytes";
        public const string BundleAssetName = "Assets/MTemp/AB/Bundle.bytes";
        public const string DependencyAssetName = "Assets/MTemp/AB/Dependency.bytes";

        #region Excel
        public static readonly string ExcelBINPath = $"{Application.streamingAssetsPath}/ExcelBIN";
        #endregion

        #region ����·��
        public static readonly string RootPath = Application.dataPath.CD();
        public static readonly string AssetPath = Application.dataPath;

        public static readonly string CorePath = @$"{RootPath}/CORE";
        #endregion

        #region ��ʱ·��
        public static readonly string TempRootPath = $"{RootPath}/MTemp";
        public static readonly string TempAssetPath = $"{Application.dataPath}/MTemp";
        #endregion

        #region ���л�·��
        public static readonly string DefaultXMLPath = $"{Environment.CurrentDirectory}/XmlSettings";
        public static readonly string DefaultJSONPath = $"{Environment.CurrentDirectory}/JsonSettings";
        public static readonly string DefaultBYTEPath = $"{Environment.CurrentDirectory}/ByteSettings";
        #endregion

        #region �����ļ�·��
        public static readonly string ABBuildSettingName = $"{CorePath}/AB/ABBuildSetting.xml";
        public static readonly string LocalizationTableName = $"{CorePath}/Localization/LocalizationTable.xlsx";
        public static readonly string LocalizationCSName = $"{AssetPath}/MFramework/Scripits/Runtime/LocalizationTable.cs";
        public static readonly string LocalizationBYTEName = $"{ExcelBINPath}/LocalizationTable.cs";
        #endregion
    }
}
