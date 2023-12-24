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
        }

        [MenuItem("MFramework/OpenFolder/OpenStreamingAssetsPath")]
        public static void OpenStreamingAssetsPath()
        {
            EditorUtility.RevealInFinder(Application.streamingAssetsPath);
            Debug.Log(Application.persistentDataPath);
        }

        [MenuItem("MFramework/OpenFolder/OpenDataPath")]
        public static void OpenDataPath()
        {
            EditorUtility.RevealInFinder(Application.dataPath);
            Debug.Log(Application.persistentDataPath);
        }

        [MenuItem("MFramework/OpenFolder/OpenTemporaryCachePath")]
        public static void OpenTemporaryCachePath()
        {
            EditorUtility.RevealInFinder(Application.temporaryCachePath);
            Debug.Log(Application.persistentDataPath);
        }

        [MenuItem("MFramework/OpenFolder/OpenConsoleLogPath")]
        public static void OpenConsoleLogPath()
        {
            EditorUtility.RevealInFinder(Application.consoleLogPath);
            Debug.Log(Application.persistentDataPath);
        }
    }
}
