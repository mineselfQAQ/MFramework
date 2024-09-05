using MFramework;
using System.Collections;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    private void Start()
    {
        UIController.Instance.OpenSponsorDisplayPanel(() =>
        {
            if (ABController.Instance.enableAB)//AB��������
            {
                //InitSync();//ͬ����ʽ
                InitAsync();//�첽��ʽ
            }
            else//һ�㳡������(ʹ��Build Settings)
            {
                MSceneUtility.LoadScene(UIController.titleScreenSceneName, () =>
                {
                    UIController.Instance.OpenTitleScreenPanel();
                    MCoroutineManager.Instance.DelayNoRecord(() =>
                    {
                        UIController.Instance.CloseSponsorDisplayPanel();
                    }, 1.0f);
                });
            }
        });
    }

    /// <summary>
    /// ͬ�����ʼ��
    /// </summary>
    private void InitSync()
    {
        //TIP������.unity�ļ�����ɿ���
        IResource resource = MResourceManager.Instance.Load($"{ABPath.ABROOTPATH}/3DGame_TitleScreen.unity", false);
        GameLoader.Instance.lastRes = resource;
        MSceneUtility.LoadScene(UIController.titleScreenSceneName, () =>
        {
            UIController.Instance.OpenTitleScreenPanel();
            MCoroutineManager.Instance.DelayNoRecord(() =>
            {
                UIController.Instance.CloseSponsorDisplayPanel();
            }, 1.0f);
        });
    }

    /// <summary>
    /// �첽���ʼ��
    /// </summary>
    private void InitAsync()
    {
        MCoroutineManager.Instance.StartCoroutine(InitAsyncCoroutine(), "STARTLOAD", () =>
        {
            MSceneUtility.LoadScene(UIController.titleScreenSceneName, () =>
            {
                UIController.Instance.OpenTitleScreenPanel();
                MCoroutineManager.Instance.DelayNoRecord(() =>
                {
                    UIController.Instance.CloseSponsorDisplayPanel();
                }, 1.0f);
            });
        });
    }

    private IEnumerator InitAsyncCoroutine()
    {
        //**���ؽ��ڴ�**
        IResource resource = MResourceManager.Instance.Load($"{ABPath.ABROOTPATH}/3DGame_TitleScreen.unity", true);
        GameLoader.Instance.lastRes = resource;
        yield return resource;//�ȴ���Դ�������
    }
}
