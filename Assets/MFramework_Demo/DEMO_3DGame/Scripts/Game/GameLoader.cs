using MFramework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// �ṩ�����л�������
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
        //����Ҫ���Я�̿�ʼ����
        if (!isLoading && (currentScene != scene))
        {
//#if UNITY_EDITOR
//          StartCoroutine(LoadRoutine(scene, onLoadFinishInternal));
//#else
            StartCoroutine(ABLoadRoutine(abPath, scene, onLoadFinishInternal));
//#endif
        }
    }
    public virtual void Reload(string abPath = null, Action onLoadFinishInternal = null)
    {
//#if UNITY_EDITOR
//      StartCoroutine(LoadRoutine(currentScene, onLoadFinishInternal));
//#else
        StartCoroutine(ABLoadRoutine(abPath, currentScene, onLoadFinishInternal));
//#endif
    }

    protected virtual IEnumerator LoadRoutine(string scene, Action onLoadFinishInternal)
    {
        OnLoadStart?.Invoke();
        isLoading = true;
        root.OpenPanel(UIController.loadPanelName);

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
        root.ClosePanel(UIController.loadPanelName);//Tip����ʱ������һ��Scene�Ѿ��������
        if (scene == UIController.titleScreenSceneName) m_controller.Disable3DScene();//�ر�3D��Ⱦ
        OnLoadFinish?.Invoke();
        onLoadFinishInternal?.Invoke();
    }

    protected virtual IEnumerator ABLoadRoutine(string abPath, string scene, Action onLoadFinishInternal)
    {
        OnLoadStart?.Invoke();
        isLoading = true;
        root.OpenPanel(UIController.loadPanelName);

        yield return new WaitForSeconds(startDelay);

        ResourceManager.Instance.Load(abPath, false);//**���ؽ��ڴ�**
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
        root.ClosePanel(UIController.loadPanelName);//Tip����ʱ������һ��Scene�Ѿ��������
        if (scene == UIController.titleScreenSceneName) m_controller.Disable3DScene();//�ر�3D��Ⱦ
        OnLoadFinish?.Invoke();
        onLoadFinishInternal?.Invoke();
    }
}