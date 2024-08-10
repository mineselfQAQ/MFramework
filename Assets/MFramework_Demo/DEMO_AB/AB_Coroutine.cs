using MFramework;
using System.Collections;
using UnityEngine;

public class AB_Coroutine : MonoBehaviour
{
    private void Start()
    {
        ResourceManager.Instance.Initialize(ABUtility.GetPlatform(), AB.GetFileUrl, 0);

        MCoroutineManager.Instance.StartCoroutine(InitAsync(), "StartLoad", () => { MLog.Print("��ʼ�����"); });
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
        yield return uiResource;//�ȴ���Դ�������
        uiResource.Instantiate();//�ڸ��ڵ��ϴ���UIRoot
        Transform uiParent = GameObject.Find("Canvas").transform;

        IResource testResource = ResourceManager.Instance.Load("Assets/AssetBundle/UI/TestUI.prefab", true);
        yield return testResource;
        testResource.Instantiate(uiParent, false);//UIRoot�µ�Canvas�´���TestUI
    }
}
