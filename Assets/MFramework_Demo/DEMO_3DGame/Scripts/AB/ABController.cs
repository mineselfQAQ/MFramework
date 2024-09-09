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

    protected string GetFileUrl(string fileName)
    {
        return $"{fileURLs[index]}/{fileName}";

        //����---D:/___UnityProject___/MFramework_AssetBundle/WINDOWS/{fileName}";
        //��λ---F:/MineselfDemo/MFramework_AssetBundle/WINDOWS/{fileName}";
    }
}