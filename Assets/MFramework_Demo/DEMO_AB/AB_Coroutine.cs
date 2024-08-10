using MFramework;
using System.Collections;
using UnityEngine;

public class AB_Coroutine : MonoBehaviour
{
    private void Start()
    {
        ResourceManager.Instance.Initialize(ABUtility.GetPlatform(), AB.GetFileUrl, 0);

        MCoroutineManager.Instance.StartCoroutine(InitAsync(), "StartLoad", () => { MLog.Print("初始化完成"); });
    }
    private void Update()
    {
        ResourceManager.Instance.Update();
    }

    private void LateUpdate()
    {
        ResourceManager.Instance.LateUpdate();
    }

    private IEnumerator InitAsync()
    {
        IResource uiResource = ResourceManager.Instance.Load("Assets/AssetBundle/UI/UIRoot.prefab", true);
        yield return uiResource;//等待资源加载完毕
        uiResource.Instantiate();//在根节点上创建UIRoot
        Transform uiParent = GameObject.Find("Canvas").transform;

        IResource testResource = ResourceManager.Instance.Load("Assets/AssetBundle/UI/TestUI.prefab", true);
        yield return testResource;
        testResource.Instantiate(uiParent, false);//UIRoot下的Canvas下创建TestUI
    }
}
