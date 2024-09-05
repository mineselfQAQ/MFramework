using MFramework;
using System.Collections;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    private void Start()
    {
        UIController.Instance.OpenSponsorDisplayPanel(() =>
        {
            if (ABController.Instance.enableAB)//AB场景加载
            {
                //InitSync();//同步形式
                InitAsync();//异步形式
            }
            else//一般场景加载(使用Build Settings)
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
    /// 同步版初始化
    /// </summary>
    private void InitSync()
    {
        //TIP：加载.unity文件会造成卡顿
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
    /// 异步版初始化
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
        //**加载进内存**
        IResource resource = MResourceManager.Instance.Load($"{ABPath.ABROOTPATH}/3DGame_TitleScreen.unity", true);
        GameLoader.Instance.lastRes = resource;
        yield return resource;//等待资源加载完毕
    }
}
