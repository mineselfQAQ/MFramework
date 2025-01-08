using MFramework;
using System.Collections.Generic;
using UnityEngine;

public class ABController : ComponentSingleton<ABController>
{
    [Tooltip("ϵͳ��Ŀ¼��AB��Ŀ¼���磺\nF:/MineselfDemo/MFramework_AssetBundle/WINDOWS")]
    public List<string> fileURLs;
    public int index;//ѡ���URL

    [Header("Settings")]
    public bool enableAB = true;

    protected override void Awake()
    {
        if (enableAB)
        {
            base.Awake();
            MResourceManager.Instance.Initialize(MABUtility.GetPlatform(), GetFileUrl, 0);
        }
    }

    protected virtual void Update()
    {
        if (enableAB)
        {
            MResourceManager.Instance.Update();
        }
    }

    protected virtual void LateUpdate()
    {
        if (enableAB)
        {
            MResourceManager.Instance.LateUpdate();
        }
    }

    protected virtual void OnApplicationQuit()
    {
        if (enableAB)
        {
            var res = GameLoader.Instance.lastRes;
            if (res != null)
            {
                MResourceManager.Instance.Unload(res);
            }
        }
    }

    protected string GetFileUrl(string fileName)
    {
        123;
        string abRootPath = MABUtility.GetABRootPath();
        return $"{abRootPath}/{fileName}";

        if (MCore.Instance.ABEncryptState)
        {
#if UNITY_EDITOR
            return $"{fileURLs[index]}_Encrypt/{fileName}";
            //����---D:/___UnityProject___/MFramework_AssetBundle/WINDOWS_Encrypt/{fileName}";
            //��λ---F:/MineselfDemo/MFramework_AssetBundle/WINDOWS_Encrypt/{fileName}";
#elif UNITY_ANDROID
        return $"{MSettings.StreamingAssetsPath}/{Application.productName}_AssetBundle/ANDROID_Encrypt/{fileName}";
#else
        return $"{MSettings.RootPath}/{Application.productName}_AssetBundle/WINDOWS_Encrypt/{fileName}";
#endif
        }
        else
        {
#if UNITY_EDITOR
            return $"{fileURLs[index]}/{fileName}";
            //����---D:/___UnityProject___/MFramework_AssetBundle/WINDOWS/{fileName}";
            //��λ---F:/MineselfDemo/MFramework_AssetBundle/WINDOWS/{fileName}";
#elif UNITY_ANDROID
        return $"{MSettings.StreamingAssetsPath}/{Application.productName}_AssetBundle/ANDROID/{fileName}";
#else
        return $"{MSettings.RootPath}/{Application.productName}_AssetBundle/WINDOWS/{fileName}";
#endif
        }
    }
}