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
    /// 同步版初始化
    /// </summary>
    private void InitSync()
    {
        //TIP：加载.unity文件会造成卡顿
        ResourceManager.Instance.Load($"{ABPath.ABROOTPATH}/3DGame_TitleScreen.unity", false);
        MSceneUtility.LoadScene(UIController.titleScreenSceneName, () =>
        {
            UIController.Instance.OpenTitleScreenPanel();
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
        IResource resource = ResourceManager.Instance.Load($"{ABPath.ABROOTPATH}/3DGame_TitleScreen.unity", true);
        yield return resource;//等待资源加载完毕
    }
}
