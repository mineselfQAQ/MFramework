using MFramework;
using UnityEngine;

public class AB_Sync : MonoBehaviour
{
    private void Start()
    {
        MResourceManager.Instance.Initialize(MABUtility.GetPlatform(), AB.GetFileUrl, 0);

        InitSync();
    }
    private void Update()
    {
        MResourceManager.Instance.Update();
    }

    private void LateUpdate()
    {
        MResourceManager.Instance.LateUpdate();
    }

    private void InitSync()
    {
        IResource uiResource = MResourceManager.Instance.Load("Assets/AssetBundle/UI/UIRoot.prefab", false);
        uiResource.Instantiate();//�ڸ��ڵ��ϴ���UIRoot
        Transform uiParent = GameObject.Find("Canvas").transform;
        IResource testResource = MResourceManager.Instance.Load("Assets/AssetBundle/UI/TestUI.prefab", false);
        testResource.Instantiate(uiParent, false);//UIRoot�µ�Canvas�´���TestUI
    }
}
