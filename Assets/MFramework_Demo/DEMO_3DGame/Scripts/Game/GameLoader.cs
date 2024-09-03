using MFramework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

/// <summary>
/// 提供场景切换功能类
/// </summary>
public class GameLoader : ComponentSingleton<GameLoader>
{
    public UnityEvent OnLoadStart;
    public UnityEvent OnLoadFinish;

    public float startDelay = 1f;
    public float finishDelay = 1f;

    public bool isLoading { get; protected set; }
    public float loadingProgress { get; protected set; }

    public string currentScene => SceneManager.GetActiveScene().name;

    protected UIController m_controller => UIController.Instance;

    protected UIRoot root;

    protected void Start()
    {
        root = m_controller.topRoot;
    }

    public virtual void Load(string scene, string abPath = null, Action onLoadFinishInternal = null)
    {
        //符合要求后，携程开始加载
        if (!isLoading && (currentScene != scene))
        {
            StartCoroutine(LoadRoutine(abPath, scene, onLoadFinishInternal));
        }
    }
    public virtual void Reload(string abPath = null, Action onLoadFinishInternal = null)
    {
        StartCoroutine(LoadRoutine(abPath, currentScene, onLoadFinishInternal));
    }

    protected virtual IEnumerator LoadRoutine(string abPath, string scene, Action onLoadFinishInternal)
    {
        OnLoadStart?.Invoke();
        isLoading = true;

        m_controller.OpenLoadingWidget();

        yield return new WaitForSeconds(startDelay);

        //开启AB
        if (ABController.Instance.enableAB)
        {
            ResourceManager.Instance.Load(abPath, false);//**加载进内存**
        }
        
        var operation = SceneManager.LoadSceneAsync(scene);//异步加载！！！
        loadingProgress = 0;//加载进度

        while (!operation.isDone)
        {
            loadingProgress = operation.progress;
            yield return null;
        }

        loadingProgress = 1;

        yield return new WaitForSeconds(finishDelay);

        isLoading = false;
        m_controller.CloseLoadingWidget();//Tip：此时背景另一个Scene已经加载完成
        if (scene == UIController.titleScreenSceneName) m_controller.Disable3DScene();//关闭3D渲染
        OnLoadFinish?.Invoke();
        onLoadFinishInternal?.Invoke();
    }
}