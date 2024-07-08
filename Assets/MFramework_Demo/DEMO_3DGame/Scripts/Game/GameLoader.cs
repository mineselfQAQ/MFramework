using MFramework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameLoader : ComponentSingleton<GameLoader>
{
    public UnityEvent OnLoadStart;
    public UnityEvent OnLoadFinish;

    public float startDelay = 1f;
    public float finishDelay = 1f;

    public bool isLoading { get; protected set; }
    public float loadingProgress { get; protected set; }

    public string currentScene => SceneManager.GetActiveScene().name;

    private UIRoot root;

    protected void Start()
    {
        root = UIController.Instance.topRoot;
    }

    public virtual void Load(string scene, Action onLoadFinishInternal = null)
    {
        //符合要求后，携程开始加载
        if (!isLoading && (currentScene != scene))
        {
            StartCoroutine(LoadRoutine(scene, onLoadFinishInternal));
        }
    }
    public virtual void Reload(Action onLoadFinishInternal = null)
    {
        StartCoroutine(LoadRoutine(currentScene, onLoadFinishInternal));
    }

    protected virtual IEnumerator LoadRoutine(string scene, Action onLoadFinishInternal)
    {
        OnLoadStart?.Invoke();
        isLoading = true;
        root.OpenPanel("Loading");

        yield return new WaitForSeconds(startDelay);

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
        root.ClosePanel("Loading");//Tip：此时背景另一个Scene已经加载完成
        OnLoadFinish?.Invoke();
        onLoadFinishInternal?.Invoke();
    }
}
