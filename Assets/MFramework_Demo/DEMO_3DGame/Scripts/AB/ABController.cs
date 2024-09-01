using MFramework;
using System.Collections;
using UnityEngine;

public class ABController : MonoBehaviour
{
    protected virtual void Start()
    {
        ResourceManager.Instance.Initialize(MABUtility.GetPlatform(), AB.GetFileUrl, 0);

        //MCoroutineManager.Instance.StartCoroutine(InitAsync(), "STARTLOAD", () => { MLog.Print("AB��ʼ�����"); });
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
        yield return uiResource;//�ȴ���Դ�������
        uiResource.Instantiate();//�ڸ��ڵ��ϴ���UIRoot
        Transform uiParent = GameObject.Find("Canvas").transform;
    }
}
