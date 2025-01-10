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

    public IResource lastRes;

    public virtual void Load(string scene, string abPath = null, Action onLoadFinishInternal = null)
    {
        //����Ҫ���Я�̿�ʼ����
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


        IResource needUnloadRes = lastRes;
        if (MCore.Instance.ABState)
        {
            //**���ؽ��ڴ�**
            //ResourceManager.Instance.Load(abPath, false);
            yield return MCoroutineManager.Instance.StartCoroutine(InitAsyncCoroutine(abPath), "STARTLOAD");
        }

        var operation = SceneManager.LoadSceneAsync(scene);//�첽���أ�����
        loadingProgress = 0;//���ؽ���

        while (!operation.isDone)
        {
            loadingProgress = operation.progress;
            yield return null;
        }

        loadingProgress = 1;

        //��ǰһ��ж�أ�Ϊж������ʱ��
        MResourceManager.Instance.Unload(needUnloadRes);
        needUnloadRes = null;

        yield return new WaitForSeconds(finishDelay);

        isLoading = false;
        m_controller.CloseLoadingWidget();//Tip����ʱ������һ��Scene�Ѿ��������
        if (scene == UIController.titleScreenSceneName) m_controller.Disable3DScene();//�ر�3D��Ⱦ
        OnLoadFinish?.Invoke();
        onLoadFinishInternal?.Invoke();
    }

    private IEnumerator InitAsyncCoroutine(string abPath)
    {
        //**���ؽ��ڴ�**
        IResource resource = MResourceManager.Instance.Load(abPath, true);
        lastRes = resource;
        yield return resource;//�ȴ���Դ�������
    }
}