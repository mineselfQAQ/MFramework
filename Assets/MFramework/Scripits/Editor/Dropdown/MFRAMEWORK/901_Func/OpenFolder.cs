using System.IO;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public class OpenFolder
    {
        [MenuItem("MFramework/OpenFolder/OpenPersistentDataPath", false, 901)]
        public static void OpenPersistentDataPath()
        {
            MEditorUtility.OpenFolder(MSettings.PersistentDataPath);
        }

        [MenuItem("MFramework/OpenFolder/OpenStreamingAssetsPath", false, 901)]
        public static void OpenStreamingAssetsPath()
        {
            MPathUtility.CreateFolderIfNotExist(MSettings.StreamingAssetsPath);
            MEditorUtility.OpenFolder(MSettings.StreamingAssetsPath);
        }

        [MenuItem("MFramework/OpenFolder/OpenDataPath", false, 901)]
        public static void OpenDataPath()
        {
            MEditorUtility.OpenFolder(MSettings.AssetPath);
        }

        [MenuItem("MFramework/OpenFolder/OpenTemporaryCachePath", false, 901)]
        public static void OpenTemporaryCachePath()
        {
            MEditorUtility.OpenFolder(MSettings.TemporaryCachePath);
        }

        [MenuItem("MFramework/OpenFolder/OpenConsoleLogPath", false, 901)]
        public static void OpenConsoleLogPath()
        {
            MEditorUtility.OpenFolder(Application.consoleLogPath);
        }

        [MenuItem("MFramework/OpenFolder/OpenCorePath", false, 912)]
        public static void OpenCorePath()
        {
            MEditorUtility.OpenFolder(MSettings.CorePath);
        }
        [MenuItem("MFramework/OpenFolder/OpenTempPath", false, 912)]
        public static void OpenTempPath()
        {
            MEditorUtility.OpenFolder(MSettings.TempAssetPath);
        }
        [MenuItem("MFramework/OpenFolder/OpenRootTempPath", false, 912)]
        public static void OpenRootTempPath()
        {
            MEditorUtility.OpenFolder(MSettings.TempRootPath);
        }
    }
}
