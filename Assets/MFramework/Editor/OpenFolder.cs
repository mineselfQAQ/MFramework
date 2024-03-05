using System.IO;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public class OpenFolder
    {
        [MenuItem("MFramework/OpenFolder/OpenPersistentDataPath")]
        public static void OpenPersistentDataPath()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
            Log.Print(Application.persistentDataPath);
        }

        [MenuItem("MFramework/OpenFolder/OpenStreamingAssetsPath")]
        public static void OpenStreamingAssetsPath()
        {
            string streamingAssetsPath = Application.streamingAssetsPath;
            if (!Directory.Exists(streamingAssetsPath))
            {
                Log.Print("当前项目不存在StreamingAssets文件夹，现已自动创建.", MLogType.Warning);
                Directory.CreateDirectory(streamingAssetsPath);
            }

            EditorUtility.RevealInFinder(streamingAssetsPath);
            Log.Print(streamingAssetsPath);
        }

        [MenuItem("MFramework/OpenFolder/OpenDataPath")]
        public static void OpenDataPath()
        {
            EditorUtility.RevealInFinder(Application.dataPath);
            Log.Print(Application.dataPath);
        }

        [MenuItem("MFramework/OpenFolder/OpenTemporaryCachePath")]
        public static void OpenTemporaryCachePath()
        {
            EditorUtility.RevealInFinder(Application.temporaryCachePath);
            Log.Print(Application.temporaryCachePath);
        }

        [MenuItem("MFramework/OpenFolder/OpenConsoleLogPath")]
        public static void OpenConsoleLogPath()
        {
            //注意：
            //consoleLogPath必须将"/"转换为"\"
            //因为Process.Start()中需要的路径使用的是"\"
            string consoleLogPath = Application.consoleLogPath;
            consoleLogPath = consoleLogPath.Replace("/", "\\");
            System.Diagnostics.Process.Start("explorer", "/select,\"" + consoleLogPath + "\"");
            Log.Print(consoleLogPath);
        }
    }
}
