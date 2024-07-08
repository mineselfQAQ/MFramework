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
        //����Ҫ���Я�̿�ʼ����
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

        var operation = SceneManager.LoadSceneAsync(scene);//�첽���أ�����
        loadingProgress = 0;//���ؽ���

        while (!operation.isDone)
        {
            loadingProgress = operation.progress;
            yield return null;
        }

        loadingProgress = 1;

        yield return new WaitForSeconds(finishDelay);

        isLoading = false;
        root.ClosePanel("Loading");//Tip����ʱ������һ��Scene�Ѿ��������
        OnLoadFinish?.Invoke();
        onLoadFinishInternal?.Invoke();
    }
}
