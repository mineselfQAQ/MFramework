using MFramework;
using UnityEngine;

public class AB_Await : MonoBehaviour
{
    private void Start()
    {
        Initialize();
    }


    private async void Initialize()
    {
        ResourceAwaiter uiRootAwaiter = MResourceManager.Instance.LoadWithAwaiter("Assets/AssetBundle/UI/UIRoot.prefab");
        await uiRootAwaiter;//û�þͼ����ȴ�
        uiRootAwaiter.GetResult().Instantiate();

        Transform uiParent = GameObject.Find("Canvas").transform;
        ResourceAwaiter testUiResource = MResourceManager.Instance.LoadWithAwaiter("Assets/AssetBundle/UI/TestUI.prefab");
        await testUiResource;
        testUiResource.GetResult().Instantiate(uiParent, false);
    }
}
