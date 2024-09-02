using MFramework;
using System.Collections;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    private void Start()
    {
        UIController.Instance.OpenSponsorDisplayPanel(() =>
        {
            //InitSync();
            InitAsync();
        });
    }

    /// <summary>
    /// ͬ�����ʼ��
    /// </summary>
    private void InitSync()
    {
        //TIP������.unity�ļ�����ɿ���
        ResourceManager.Instance.Load($"{ABPath.ABROOTPATH}/3DGame_TitleScreen.unity", false);
        MSceneUtility.LoadScene(UIController.titleScreenSceneName, () =>
        {
            UIController.Instance.OpenTitleScreenPanel();
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
        IResource resource = ResourceManager.Instance.Load($"{ABPath.ABROOTPATH}/3DGame_TitleScreen.unity", true);
        yield return resource;//�ȴ���Դ�������
    }
}
