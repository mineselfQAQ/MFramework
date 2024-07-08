using MFramework;
using UnityEngine;

public class AB_Sync : MonoBehaviour
{
    private void Start()
    {
        ResourceManager.Instance.Initialize(ABUtility.GetPlatform(), GetFileUrl, 0);

        InitSync();
    }
    private void Update()
    {
        ResourceManager.Instance.Update();
    }

    private void LateUpdate()
    {
        ResourceManager.Instance.LateUpdate();
    }

    private void InitSync()
    {
        IResource uiResource = ResourceManager.Instance.Load("Assets/AssetBundle/UI/UIRoot.prefab", false);
        uiResource.Instantiate();//在根节点上创建UIRoot
        Transform uiParent = GameObject.Find("Canvas").transform;
        IResource testResource = ResourceManager.Instance.Load("Assets/AssetBundle/UI/TestUI.prefab", false);
        testResource.Instantiate(uiParent, false);//UIRoot下的Canvas下创建TestUI
    }

    private string GetFileUrl(string fileName)
    {
        return $"F:/___MYPROJECT___/UnityProject/MFramework_AssetBundle/WINDOWS/{fileName}";
    }
}
