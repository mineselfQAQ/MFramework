using MFramework;
using System.Collections;
using UnityEngine;

public class AB_Coroutine : MonoBehaviour
{
    private void Start()
    {
        MResourceManager.Instance.Initialize(MABUtility.GetPlatform(), AB.GetFileUrl, 0);

        MCoroutineManager.Instance.StartCoroutine(InitAsync(), "StartLoad", () => { MLog.Print("��ʼ�����"); });
    }
    private void Update()
    {
        MResourceManager.Instance.Update();
    }

    private void LateUpdate()
    {
        MResourceManager.Instance.LateUpdate();
    }

    private IEnumerator InitAsync()
    {
        IResource uiResource = MResourceManager.Instance.Load("Assets/AssetBundle/UI/UIRoot.prefab", true);
        yield return uiResource;//�ȴ���Դ�������
        uiResource.Instantiate();//�ڸ��ڵ��ϴ���UIRoot
        Transform uiParent = GameObject.Find("Canvas").transform;

        IResource testResource = MResourceManager.Instance.Load("Assets/AssetBundle/UI/TestUI.prefab", true);
        yield return testResource;
        testResource.Instantiate(uiParent, false);//UIRoot�µ�Canvas�´���TestUI
    }
}
