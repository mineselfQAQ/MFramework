using MFramework;
using UnityEngine;

public class AB_Await : MonoBehaviour
{
    private void Start()
    {
        ResourceManager.Instance.Initialize(MABUtility.GetPlatform(), AB.GetFileUrl, 0);

        Initialize();
    }


    private async void Initialize()
    {
        ResourceAwaiter uiRootAwaiter = ResourceManager.Instance.LoadWithAwaiter("Assets/AssetBundle/UI/UIRoot.prefab");
        await uiRootAwaiter;//没好就继续等待
        uiRootAwaiter.GetResult().Instantiate();

        Transform uiParent = GameObject.Find("Canvas").transform;
        ResourceAwaiter testUiResource = ResourceManager.Instance.LoadWithAwaiter("Assets/AssetBundle/UI/TestUI.prefab");
        await testUiResource;
        testUiResource.GetResult().Instantiate(uiParent, false);
    }

    private void Update()
    {
        ResourceManager.Instance.Update();
    }

    private void LateUpdate()
    {
        ResourceManager.Instance.LateUpdate();
    }
}
