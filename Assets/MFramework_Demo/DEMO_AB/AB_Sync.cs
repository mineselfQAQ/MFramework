using MFramework;
using UnityEngine;

public class AB_Sync : MonoBehaviour
{
    private void InitSync()
    {
        IResource uiResource = MResourceManager.Instance.Load("Assets/AssetBundle/UI/UIRoot.prefab", false);
        uiResource.Instantiate();//在根节点上创建UIRoot
        Transform uiParent = GameObject.Find("Canvas").transform;
        IResource testResource = MResourceManager.Instance.Load("Assets/AssetBundle/UI/TestUI.prefab", false);
        testResource.Instantiate(uiParent, false);//UIRoot下的Canvas下创建TestUI
    }
}
