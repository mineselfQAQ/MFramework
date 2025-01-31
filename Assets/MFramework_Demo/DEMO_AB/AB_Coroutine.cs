using MFramework;
using System.Collections;
using UnityEngine;

public class AB_Coroutine : MonoBehaviour
{
    private void Start()
    {
        MCoroutineManager.Instance.StartCoroutine(InitAsync(), "StartLoad", () => { MLog.Print("初始化完成"); });
    }

    private IEnumerator InitAsync()
    {
        IResource uiResource = MResourceManager.Instance.Load("Assets/AssetBundle/UI/UIRoot.prefab", true);
        yield return uiResource;//等待资源加载完毕
        uiResource.Instantiate();//在根节点上创建UIRoot
        Transform uiParent = GameObject.Find("Canvas").transform;

        IResource testResource = MResourceManager.Instance.Load("Assets/AssetBundle/UI/TestUI.prefab", true);
        yield return testResource;
        testResource.Instantiate(uiParent, false);//UIRoot下的Canvas下创建TestUI
    }
}
