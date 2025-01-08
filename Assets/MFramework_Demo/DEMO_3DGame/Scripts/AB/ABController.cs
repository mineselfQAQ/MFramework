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
        string abRootPath = null;
#if UNITY_EDITOR
        if (MCore.Instance.ABEncryptState)
        {
            abRootPath = $"{fileURLs[index]}_ENCRYPT";
        }
        else
        {
            abRootPath = $"{fileURLs[index]}";
        }
#else
        abRootPath = MABUtility.GetABRootPath();
#endif

        return $"{abRootPath}/{fileName}";
    }
}