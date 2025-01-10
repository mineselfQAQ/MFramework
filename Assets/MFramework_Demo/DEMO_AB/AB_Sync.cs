using MFramework;
using UnityEngine;

public class AB_Sync : MonoBehaviour
{
    private void InitSync()
    {
        IResource uiResource = MResourceManager.Instance.Load("Assets/AssetBundle/UI/UIRoot.prefab", false);
        uiResource.Instantiate();//�ڸ��ڵ��ϴ���UIRoot
        Transform uiParent = GameObject.Find("Canvas").transform;
        IResource testResource = MResourceManager.Instance.Load("Assets/AssetBundle/UI/TestUI.prefab", false);
        testResource.Instantiate(uiParent, false);//UIRoot�µ�Canvas�´���TestUI
    }
}
