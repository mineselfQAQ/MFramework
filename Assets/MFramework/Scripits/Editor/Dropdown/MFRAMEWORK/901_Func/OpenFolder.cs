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
            EditorUtility.RevealInFinder(Application.persistentDataPath);
            MLog.Print($"PersistentDataPath路径：{Application.persistentDataPath}");
        }

        [MenuItem("MFramework/OpenFolder/OpenStreamingAssetsPath", false, 901)]
        public static void OpenStreamingAssetsPath()
        {
            string streamingAssetsPath = Application.streamingAssetsPath;
            if (!Directory.Exists(streamingAssetsPath))
            {
                Directory.CreateDirectory(streamingAssetsPath);
            }

            EditorUtility.RevealInFinder(streamingAssetsPath);
            MLog.Print(streamingAssetsPath);
        }

        [MenuItem("MFramework/OpenFolder/OpenDataPath", false, 901)]
        public static void OpenDataPath()
        {
            EditorUtility.RevealInFinder(Application.dataPath);
            MLog.Print($"DataPath路径：{Application.dataPath}");
        }

        [MenuItem("MFramework/OpenFolder/OpenTemporaryCachePath", false, 901)]
        public static void OpenTemporaryCachePath()
        {
            EditorUtility.RevealInFinder(Application.temporaryCachePath);
            MLog.Print($"TemporaryCachePath路径：{Application.temporaryCachePath}");
        }

        [MenuItem("MFramework/OpenFolder/OpenConsoleLogPath", false, 901)]
        public static void OpenConsoleLogPath()
        {
            //注意：
            //consoleLogPath必须将"/"转换为"\"
            //因为Process.Start()中需要的路径使用的是"\"
            string consoleLogPath = Application.consoleLogPath;
            consoleLogPath = consoleLogPath.Replace("/", "\\");
            System.Diagnostics.Process.Start("explorer", "/select,\"" + consoleLogPath + "\"");
            MLog.Print($"ConsoleLogPath路径：{Application.consoleLogPath}");
        }
    }
}
