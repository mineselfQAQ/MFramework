using UnityEngine;

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
                    MLog.Print($"δ֧�ֵ�ƽ̨:{Application.platform}", MLogType.Error);
                    return null;
            }
        }

        //TODO��ֻ֧�ְ�׿/����(ָPC)
        public static string GetABRootPath()
        {
            if (MCore.Instance.ABEncryptState)
            {
#if UNITY_ANDROID
        return $"{MSettings.StreamingAssetsPath}/{Application.productName}_AssetBundle/ANDROID_Encrypt";
#else
        return $"{MSettings.RootPath}/{Application.productName}_AssetBundle/WINDOWS_Encrypt";
#endif
            }
            else
            {
#if UNITY_ANDROID
        return $"{MSettings.StreamingAssetsPath}/{Application.productName}_AssetBundle/ANDROID";
#else
        return $"{MSettings.RootPath}/{Application.productName}_AssetBundle/WINDOWS";
#endif
            }
        }

        internal static AssetBundle LoadAB(string file)
        {
            bool isABFile = false;
            if (file.Contains(".ab"))
            {
                isABFile = true;
            }
            //�����ļ�(��Unity�ṩ)
            if (MCore.Instance.ABEncryptState && isABFile)
            {
                byte[] data = AESUtlity.AESDecryptFileToStream(file);
                if (data == null)
                {
                    MLog.Print($"{typeof(MABUtility)}��{file}����ʧ�ܣ�����", MLogType.Error);
                }
                return AssetBundle.LoadFromMemory(data, 0);
            }
            else
            {
                return AssetBundle.LoadFromFile(file, 0, MBundleManager.Instance.offset);
            }
        }
        internal static AssetBundleCreateRequest LoadABAsync(string file)
        {
            bool isABFile = false;
            if (file.Contains(".ab"))
            {
                isABFile = true;
            }
            //�����ļ�(��Unity�ṩ)
            if (MCore.Instance.ABEncryptState && isABFile)
            {
                byte[] data = AESUtlity.AESDecryptFileToStream(file);
                if (data == null)
                {
                    MLog.Print($"{typeof(MABUtility)}��{file}����ʧ�ܣ�����", MLogType.Error);
                }
                return AssetBundle.LoadFromMemoryAsync(data, 0);
            }
            else
            {
                return AssetBundle.LoadFromFileAsync(file, 0, MBundleManager.Instance.offset);
            }
        }
    }
}