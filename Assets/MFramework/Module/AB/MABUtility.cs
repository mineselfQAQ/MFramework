using UnityEngine;
using MFramework.Core;

using MFramework.Util;
namespace MFramework
{
    public static class MABUtility
    {
        public static string GetPlatform()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "WINDOWS";
                case RuntimePlatform.Android:
                    return "ANDROID";
                case RuntimePlatform.IPhonePlayer:
                    return "IOS";
                default:
                    MLog.Default?.E("AB error.");
                    return null;
            }
        }

        // TODO：只支持安卓/PC（默认状态下）
        public static string GetABRootPath(bool encrypt)
        {
            if (encrypt)
            {
#if UNITY_STANDALONE
                return $"{Application.dataPath.CD()}/{Application.productName}_AssetBundle/WINDOWS_ENCRYPT";
#elif UNITY_ANDROID
                return $"{Application.streamingAssetsPath}/{Application.productName}_AssetBundle/ANDROID_ENCRYPT";
#elif UNITY_IOS
                return null;
#else
                return null;
#endif
            }
            else
            {
#if UNITY_STANDALONE
                return $"{Application.dataPath.CD()}/{Application.productName}_AssetBundle/WINDOWS";
#elif UNITY_ANDROID
                return $"{Application.streamingAssetsPath}/{Application.productName}_AssetBundle/ANDROID";
#elif UNITY_IOS
                return null;
#else
                return null;
#endif
            }
        }

        public static string GetABLine(string name, string md5, string size)
        {
            // 注意：不应该用空格，会出现如textmesh pro之类的文件夹干扰
            return $"{name}|{md5}|{size}";
        }
        public static string GetABLine(string name, string md5, int size)
        {
            // 注意：不应该用空格，会出现如textmesh pro之类的文件夹干扰
            return $"{name}|{md5}|{size}";
        }

        public static AssetBundle LoadAB(string file, ABRuntimeState state, ulong offset)
        {
            bool isABFile = false;
            if (file.Contains(".ab"))
            {
                isABFile = true;
            }
            // 加载文件（由Unity提供）
            if (state != null && state.ABEncryptState && isABFile)
            {
                byte[] data = AESUtlity.AESDecryptFileToStream(file);
                if (data == null)
                {
                    MLog.Default?.E("AB error.");
                }
                return AssetBundle.LoadFromMemory(data, 0);
            }
            else
            {
                return AssetBundle.LoadFromFile(file, 0, offset);
            }
        }
        public static AssetBundleCreateRequest LoadABAsync(string file, ABRuntimeState state, ulong offset)
        {
            bool isABFile = false;
            if (file.Contains(".ab"))
            {
                isABFile = true;
            }
            // 加载文件（由Unity提供）
            if (state != null && state.ABEncryptState && isABFile)
            {
                byte[] data = AESUtlity.AESDecryptFileToStream(file);
                if (data == null)
                {
                    MLog.Default?.E("AB error.");
                }
                return AssetBundle.LoadFromMemoryAsync(data, 0);
            }
            else
            {
                return AssetBundle.LoadFromFileAsync(file, 0, offset);
            }
        }
    }
}