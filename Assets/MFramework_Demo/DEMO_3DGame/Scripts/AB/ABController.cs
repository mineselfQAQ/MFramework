using MFramework;
using System.Collections;
using UnityEngine;

public class ABController : MonoBehaviour
{
    protected virtual void Start()
    {
        ResourceManager.Instance.Initialize(MABUtility.GetPlatform(), AB.GetFileUrl, 0);

        //MCoroutineManager.Instance.StartCoroutine(InitAsync(), "STARTLOAD", () => { MLog.Print("AB初始化完成"); });
    }
    protected virtual void Update()
    {
        ResourceManager.Instance.Update();
    }

    protected virtual void LateUpdate()
    {
        ResourceManager.Instance.LateUpdate();
    }

    protected virtual IEnumerator InitAsync()
    {
        IResource uiResource = ResourceManager.Instance.Load("Assets/AssetBundle/UI/UIRoot.prefab", true);
        yield return uiResource;//等待资源加载完毕
        uiResource.Instantiate();//在根节点上创建UIRoot
        Transform uiParent = GameObject.Find("Canvas").transform;
    }
}
