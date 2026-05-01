using System.IO;
using UnityEngine;

namespace MFramework.Core
{
    public static class MPathCache
    {
        public static readonly string ASSET_PATH = Application.dataPath;
        public static readonly string STREAMING_ASSETS_PATH = Application.streamingAssetsPath;
        public static readonly string PERSISTENT_DATA_PATH = Application.persistentDataPath;
        public static readonly string TEMPORARY_CACHE_PATH = Application.temporaryCachePath;

        /// <summary>
        /// PC根目录(包括Editor/Build)
        /// </summary>
        public static readonly string PC_ROOT_PATH = Path.GetDirectoryName(ASSET_PATH);

        public static readonly string DEFAULT_XML_PATH = $"{PERSISTENT_DATA_PATH}/XmlSettings";
        public static readonly string DEFAULT_JSON_PATH = $"{PERSISTENT_DATA_PATH}/JsonSettings";
        public static readonly string DEFAULT_BYTE_PATH = $"{PERSISTENT_DATA_PATH}/ByteSettings";

        public static readonly string CORE_PATH = $"{PC_ROOT_PATH}/CORE";
        public static readonly string TEMP_ROOT_PATH = $"{PC_ROOT_PATH}/MTemp";
        public static readonly string TEMP_ASSET_PATH = $"{ASSET_PATH}/MTemp";

        public static readonly string AB_BUILD_SETTING_PATH = $"{CORE_PATH}/AB/ABBuildSetting.xml";
        public const string AB_INFO_FILE_NAME = "ABInfo.txt";
        public const string AB_RESOURCE_ASSET_NAME = "Assets/MTemp/AB/Resource.bytes";
        public const string AB_BUNDLE_ASSET_NAME = "Assets/MTemp/AB/Bundle.bytes";
        public const string AB_DEPENDENCY_ASSET_NAME = "Assets/MTemp/AB/Dependency.bytes";
    }
}
