using System.IO;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public class OpenFolder
    {
        [MenuItem("MFramework/OpenFolder/OpenPersistentDataPath", false, 1000)]
        public static void OpenPersistentDataPath()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
            MLog.Print(Application.persistentDataPath);
        }

        [MenuItem("MFramework/OpenFolder/OpenStreamingAssetsPath", false, 1000)]
        public static void OpenStreamingAssetsPath()
        {
            string streamingAssetsPath = Application.streamingAssetsPath;
            if (!Directory.Exists(streamingAssetsPath))
            {
                MLog.Print("当前项目不存在StreamingAssets文件夹，现已自动创建.", MLogType.Warning);
                Directory.CreateDirectory(streamingAssetsPath);
            }

            EditorUtility.RevealInFinder(streamingAssetsPath);
            MLog.Print(streamingAssetsPath);
        }

        [MenuItem("MFramework/OpenFolder/OpenDataPath", false, 1000)]
        public static void OpenDataPath()
        {
            EditorUtility.RevealInFinder(Application.dataPath);
            MLog.Print(Application.dataPath);
        }

        [MenuItem("MFramework/OpenFolder/OpenTemporaryCachePath", false, 1000)]
        public static void OpenTemporaryCachePath()
        {
            EditorUtility.RevealInFinder(Application.temporaryCachePath);
            MLog.Print(Application.temporaryCachePath);
        }

        [MenuItem("MFramework/OpenFolder/OpenConsoleLogPath", false, 1000)]
        public static void OpenConsoleLogPath()
        {
            //注意：
            //consoleLogPath必须将"/"转换为"\"
            //因为Process.Start()中需要的路径使用的是"\"
            string consoleLogPath = Application.consoleLogPath;
            consoleLogPath = consoleLogPath.Replace("/", "\\");
            System.Diagnostics.Process.Start("explorer", "/select,\"" + consoleLogPath + "\"");
            MLog.Print(consoleLogPath);
        }
    }
}
