using MFramework;
using UnityEngine;

public class ABController : ComponentSingleton<ABController>
{
    [Tooltip("系统根目录至AB根目录，如：\nF:/MineselfDemo/MFramework_AssetBundle/WINDOWS")]
    public string fileURL;

    [Header("Settings")]
    public bool enableAB = true;

    protected override void Awake()
    {
        if (enableAB)
        {
            base.Awake();
            ResourceManager.Instance.Initialize(MABUtility.GetPlatform(), GetFileUrl, 0);
        }
    }

    protected virtual void Update()
    {
        if (enableAB)
        {
            ResourceManager.Instance.Update();
        }
    }

    protected virtual void LateUpdate()
    {
        if (enableAB)
        {
            ResourceManager.Instance.LateUpdate();
        }
    }

    protected string GetFileUrl(string fileName)
    {
        //return $"{fileURL}/{fileName}";
        return $"D:/___UnityProject___/MFramework_AssetBundle/WINDOWS/{fileName}";
        //return $"F:/MineselfDemo/MFramework_AssetBundle/WINDOWS/{fileName}";
    }
}