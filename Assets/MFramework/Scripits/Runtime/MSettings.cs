using UnityEngine;

namespace MFramework
{
    public class MSettings
    {
        public const string MCoreName = "#MCORE#";
        public const string UICanvasName = "UICanvas";
        public const string UICameraName = "UICamera";

        public const string ResourceAssetName = "Assets/MTemp/AB/Resource.bytes";
        public const string BundleAssetName = "Assets/MTemp/AB/Bundle.bytes";
        public const string DependencyAssetName = "Assets/MTemp/AB/Dependency.bytes";

        #region Excel
        public static readonly string ExcelBINPath = $"{Application.streamingAssetsPath}/ExcelBIN";
        #endregion

        #region 基础路径
        public static readonly string RootPath = Application.dataPath.CD();
        public static readonly string AssetPath = Application.dataPath;
        public static readonly string StreamingAssetsPath = Application.streamingAssetsPath;
        public static readonly string PersistentDataPath = Application.persistentDataPath;
        public static readonly string TemporaryCachePath = Application.temporaryCachePath;

        public static readonly string CorePath = @$"{RootPath}/CORE";
        #endregion

        #region 临时路径
        public static readonly string TempRootPath = $"{RootPath}/MTemp";
        public static readonly string TempAssetPath = $"{AssetPath}/MTemp";
        #endregion

        #region 序列化路径
        public static readonly string DefaultXMLPath = $"{PersistentDataPath}/XmlSettings";
        public static readonly string DefaultJSONPath = $"{PersistentDataPath}/JsonSettings";
        public static readonly string DefaultBYTEPath = $"{PersistentDataPath}/ByteSettings";
        #endregion

        #region 具体文件路径
        public static readonly string ABBuildSettingName = $"{CorePath}/AB/ABBuildSetting.xml";

        public static readonly string LocalizationTableName = $"{CorePath}/Localization/LocalizationTable.xlsx";
        public static readonly string LocalizationCSName = $"{AssetPath}/MFramework/Scripits/Runtime/LocalizationTable.cs";
        public static readonly string LocalizationBYTEName = $"{StreamingAssetsPath}/LocalizationTable.byte";
        public const string LocalizationLoadBINName = "{Application.streamingAssetsPath}/LocalizationTable.byte";
        #endregion
    }
}
