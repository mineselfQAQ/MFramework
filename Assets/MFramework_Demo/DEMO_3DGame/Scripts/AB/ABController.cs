using MFramework;
using System.Collections.Generic;
using UnityEngine;

public class ABController : ComponentSingleton<ABController>
{
    [Tooltip("系统根目录至AB根目录，如：\nF:/MineselfDemo/MFramework_AssetBundle/WINDOWS")]
    public List<string> fileURLs;
    public int index;//选择的URL

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
#if UNITY_EDITOR
        Debug.Log($"{fileURLs[index]}/{fileName}");
        return $"{fileURLs[index]}/{fileName}";
        //家中---D:/___UnityProject___/MFramework_AssetBundle/WINDOWS/{fileName}";
        //单位---F:/MineselfDemo/MFramework_AssetBundle/WINDOWS/{fileName}";
#else
        return $"{MSettings.RootPath}/{Application.productName}_AssetBundle/WINDOWS/{fileName}";
#endif
    }
}