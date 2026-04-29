using System.IO;
using UnityEngine;

namespace MFramework.Core
{
    public static class MPathCache
    {
        /// <summary>
        /// PC根目录(包括Editor/Build)
        /// </summary>
        public static readonly string PC_ROOT_PATH = Path.GetDirectoryName(Application.dataPath);

        public static readonly string ASSET_PATH = Application.dataPath;
        public static readonly string STREAMING_ASSETS_PATH = Application.streamingAssetsPath;
        public static readonly string PERSISTENT_DATA_PATH = Application.persistentDataPath;
        public static readonly string TEMPORARY_CACHE_PATH = Application.temporaryCachePath;

        public static readonly string DEFAULT_XML_PATH = $"{PERSISTENT_DATA_PATH}/XmlSettings";
        public static readonly string DEFAULT_JSON_PATH = $"{PERSISTENT_DATA_PATH}/JsonSettings";
        public static readonly string DEFAULT_BYTE_PATH = $"{PERSISTENT_DATA_PATH}/ByteSettings";
    }
}
