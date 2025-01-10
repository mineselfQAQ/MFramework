using MFramework;
using System.Collections;
using UnityEngine;

public class InitPanel : InitPanelBase
{
    private float totalSize;

    public override void Init()
    {
        m_MSlider_Slider.value = 0;

        totalSize = MHotUpdateManager.Instance.downloadTotalSize;
    }

    public override void Update()
    {
        MCoroutineManager.Instance.Loop("DownloadLoop", () =>
        {
            float curSize = MHotUpdateManager.Instance.curDownloadSize;
            m_MSlider_Slider.value = curSize / totalSize;
        }, 0, 1);

        //热更新结束后关闭协程并进入下一界面
        MCoroutineManager.Instance.WaitNoRecord(() =>
        {
            MCoroutineManager.Instance.EndCoroutine("DownloadLoop");

            if (MCore.Instance.ABState)//AB场景加载
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
                        UIController.Instance.DestroyInitPanel();
                    }, 1.0f);
                });
            }
        }, MCore.Instance.isHotUpdateFinish);
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
                UIController.Instance.CloseInitPanel();
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
                    UIController.Instance.CloseInitPanel();
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

    protected override GameObject LoadPrefab(string prefabPath)
    {
        return ABUtility.LoadPanelSync(prefabPath);
    }
}