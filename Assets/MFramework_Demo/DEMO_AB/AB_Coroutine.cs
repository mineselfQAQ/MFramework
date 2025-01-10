using MFramework;
using System.Collections;
using UnityEngine;

public class AB_Coroutine : MonoBehaviour
{
    private void Start()
    {
        MCoroutineManager.Instance.StartCoroutine(InitAsync(), "StartLoad", () => { MLog.Print("��ʼ�����"); });
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
