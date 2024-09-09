using MFramework;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class UIController : ComponentSingleton<UIController>
{
    [Header("SponsorDisplayPanel Settings")]
    public float stayTime = 2.0f;

    [Header("HUD Settings")]
    public string HUDRetriesFormat = "00";
    public string HUDCoinsFormat = "000";
    public string HUDHealthFormat = "0";

    [Header("Flash Settings")]
    public float flashDuration = 1.0f;

    public UIRoot bottomRoot;
    public UIRoot topRoot;

    public Dictionary<string, UIPanel> panelDic;

    public static readonly string panelPrepath = "Assets/MFramework_Demo/DEMO_3DGame/MFrameworkUI/Panels";
    public static readonly string widgetPrepath = "Assets/MFramework_Demo/DEMO_3DGame/MFrameworkUI/Widgets";

    public static readonly string sponsorDisplayPanelName = "SPONSOR";
    public static readonly string loadPanelName = "LOADING";
    public static readonly string pausePanelName = "PAUSE";
    public static readonly string transitionPanelName = "TRANSITION";
    public static readonly string HUDPanelName = "HUD";
    public static readonly string titleScreenPanelName = "TITLESCREEN";
    public static readonly string fileSelectPanelName = "FILESELECT";
    public static readonly string levelSelectPanelName = "LEVELSELECT";
    public static readonly string dialogPanelName = "DIALOG";

    public static readonly string flashEffectName = "FLASHEFFECT";

    public static readonly string titleScreenSceneName = "3DGame_TitleScreen";
    public static readonly string starterSceneName = "3DGame_Starter";

    protected FlashEffect flashEffect;

    protected override void Awake()
    {
        base.Awake();

        //UIManager.Instance.SetDontDestroyOnLoad();
        panelDic = new Dictionary<string, UIPanel>();

        bottomRoot = UIManager.Instance.CreateRoot("BOTTOMROOT", 0, 999);
        topRoot = UIManager.Instance.CreateRoot("TOPROOT", 1000, 1999);

        flashEffect = (FlashEffect)CreatePanel<FlashEffect>(topRoot, flashEffectName, $"{panelPrepath}/FlashEffect/FlashEffect.prefab", false);
        topRoot.SetSortingOrder(flashEffectName, 1994);
        CreatePanel<DialogPanel>(topRoot, dialogPanelName, $"{panelPrepath}/DialogPanel/DialogPanel.prefab", false);
        topRoot.SetSortingOrder(dialogPanelName, 1995);
        CreatePanel<PausePanel>(topRoot, pausePanelName, $"{panelPrepath}/PausePanel/PausePanel.prefab", false);
        topRoot.SetSortingOrder(pausePanelName, 1996);
        CreatePanel<TransitionPanel>(topRoot, transitionPanelName, $"{panelPrepath}/TransitionPanel/TransitionPanel.prefab", false);
        topRoot.SetSortingOrder(transitionPanelName, 1997);
        CreatePanel<SponsorDisplayPanel>(topRoot, sponsorDisplayPanelName, $"{panelPrepath}/SponsorDisplayPanel/SponsorDisplayPanel.prefab", false);
        topRoot.SetSortingOrder(sponsorDisplayPanelName, 1998);

        //其它场景直接启动(用于测试)不创建TitleScreenPanel
#if UNITY_EDITOR
        if (EditorSceneManager.GetActiveScene().name != starterSceneName)
        {
            return;
        }
#endif
        //创建TitleScreenPanel
        CreatePanel<TitleScreenPanel>(bottomRoot, titleScreenPanelName, $"{panelPrepath}/TitleScreenPanel/TitleScreenPanel.prefab", false);
    }

    protected void Update()
    {
        foreach (var panel in panelDic.Values)
        {
            panel.Update();
        }
    }

    public UIPanel CreatePanel<T>(UIRoot root, string id, string prefabPath, bool autoEnter) where T : UIPanel
    {
        if (panelDic.ContainsKey(id)) 
        {
            root.DestroyPanel(id);
            panelDic.Remove(id);
        }

        UIPanel panel = root.CreatePanel<T>(id, prefabPath, autoEnter);
        panel.Init();
        panelDic.Add(id, panel);

        return panel;
    }

    public void DestroyPanel(UIRoot root, string id)
    {
        if (panelDic.ContainsKey(id))
        {
            root.DestroyPanel(id);
            panelDic.Remove(id);
        }
    }

    #region 具体功能函数
    #region 对话面板
    public DialogPanel GetDialogPanel()
    {
        return (DialogPanel)panelDic[dialogPanelName];
    }
    #endregion
    #region 过渡面板
    public void OpenTakeBloodRestartWidget(Action onFinish = null)
    {
        topRoot.OpenPanel(transitionPanelName);
        topRoot.GetPanel<TransitionPanel>(transitionPanelName).OpenTakeBloodRestartWidget(onFinish);
    }
    public void CloseTakeBloodRestartWidget(Action onFinish = null)
    {
        topRoot.GetPanel<TransitionPanel>(transitionPanelName).CloseTakeBloodRestartWidget(() =>
        {
            topRoot.ClosePanel(transitionPanelName, onFinish);
        });
    }

    public void OpenRestartWidget(Action onFinish = null)
    {
        topRoot.OpenPanel(transitionPanelName);
        topRoot.GetPanel<TransitionPanel>(transitionPanelName).OpenRestartWidget(onFinish);
    }
    public void CloseRestartWidget(Action onFinish = null)
    {
        topRoot.GetPanel<TransitionPanel>(transitionPanelName).CloseRestartWidget(() =>
        {
            topRoot.ClosePanel(transitionPanelName, onFinish);
        });
    }

    public void OpenGameOverWidget(Action onFinish = null)
    {
        topRoot.OpenPanel(transitionPanelName);
        topRoot.GetPanel<TransitionPanel>(transitionPanelName).OpenGameOverWidget(onFinish);
    }
    public void CloseGameOverWidget(Action onFinish = null)
    {
        topRoot.GetPanel<TransitionPanel>(transitionPanelName).CloseGameOverWidget(() =>
        {
            topRoot.ClosePanel(transitionPanelName, onFinish);
        });
    }
    /// <summary>
    /// 关闭CloseGameOverWidget(但不关闭TransitionPanel)
    /// </summary>
    public void OnlyCloseGameOverWidget(Action onFinish = null)
    {
        topRoot.GetPanel<TransitionPanel>(transitionPanelName).CloseGameOverWidget();
    }

    public void OpenLoadingWidget(Action onFinish = null)
    {
        topRoot.OpenPanel(transitionPanelName);
        topRoot.GetPanel<TransitionPanel>(transitionPanelName).OpenLoadingWidget(onFinish);
    }
    public void CloseLoadingWidget(Action onFinish = null)
    {
        topRoot.GetPanel<TransitionPanel>(transitionPanelName).CloseLoadingWidget(() =>
        {
            topRoot.ClosePanel(transitionPanelName, onFinish);
        });
    }
    #endregion
    #region 支持者面板
    public void OpenSponsorDisplayPanel(Action onFinish = null)
    {
        topRoot.OpenPanel(sponsorDisplayPanelName, onFinish);
    }
    public void CloseSponsorDisplayPanel(Action onFinish = null)
    {
        topRoot.ClosePanel(sponsorDisplayPanelName, onFinish);
    }
    #endregion
    #region 标题面板
    public void OpenTitleScreenPanel(Action onFinish = null)
    {
        bottomRoot.OpenPanel(titleScreenPanelName, onFinish);
    }
    public void CloseTitleScreenPanel(Action onFinish = null)
    {
        bottomRoot.ClosePanel(titleScreenPanelName, onFinish);
    }
    #endregion
    #region 暂停面板
    public void OpenPausePanel(Action onFinish = null)
    {
        topRoot.OpenPanel(pausePanelName, onFinish);
    }
    public void ClosePausePanel(Action onFinish = null)
    {
        topRoot.ClosePanel(pausePanelName, onFinish);
    }
    #endregion
    #region 存档选择面板
    public void OpenFileSelectPanel(Action onFinish = null)
    {
        ((FileSelectPanel)panelDic[fileSelectPanelName]).Refresh();
        bottomRoot.OpenPanel(fileSelectPanelName, onFinish);
    }
    public void CloseFileSelectPanel(Action onFinish = null)
    {
        bottomRoot.ClosePanel(fileSelectPanelName, onFinish);
    }
    #endregion
    #region HUD面板
    public void CreateOrOpenHUD(Action onFinish = null)
    {
        if (!topRoot.ExistPanel(HUDPanelName))
        {
            CreatePanel<HUDPanel>(topRoot, HUDPanelName, $"{panelPrepath}/HUDPanel/HUDPanel.prefab", true);
        }
        else
        {
            ((HUDPanel)panelDic[HUDPanelName]).ResetPanel();
            topRoot.OpenPanel(HUDPanelName, onFinish);
        }
    }
    public void CloseHUD(Action onFinish = null)
    {
        topRoot.ClosePanel(HUDPanelName, onFinish);
    }
    #endregion 
    #region 特定指向
    public void TitleScreenToFileSelect()
    {
        bottomRoot.ClosePanel(titleScreenPanelName);

        if (!panelDic.ContainsKey(fileSelectPanelName))
        {
            CreatePanel<FileSelectPanel>(bottomRoot, fileSelectPanelName, $"{panelPrepath}/FileSelectPanel/FileSelectPanel.prefab", true);
        }
        else
        {
            bottomRoot.OpenPanel(fileSelectPanelName);
        }

        Disable3DScene();
    }

    public void FileSelectToLevelSelect()
    {
        bottomRoot.ClosePanel(fileSelectPanelName);


        if (!panelDic.ContainsKey(levelSelectPanelName))
        {
            CreatePanel<LevelSelectPanel>(bottomRoot, levelSelectPanelName, $"{panelPrepath}/LevelSelectPanel/LevelSelectPanel.prefab", true);
        }
        else
        {
            bottomRoot.OpenPanel(levelSelectPanelName);
            ((LevelSelectPanel)panelDic[levelSelectPanelName]).Refresh();
        }
    }

    public void LevelSelectBackToFileSelect()
    {
        bottomRoot.ClosePanel(levelSelectPanelName);
        bottomRoot.OpenPanel(fileSelectPanelName);
        ((FileSelectPanel)panelDic[fileSelectPanelName]).Refresh();
    }
    #endregion
    #region 特效
    public void TriggerFlash()
    {
        topRoot.OpenPanel(flashEffectName);

        flashEffect.Flash(flashDuration, () =>
        {
            topRoot.ClosePanel(flashEffectName);
        });
    }
    #endregion
    #endregion

    /// <summary>
    /// 不再显示场景中的3D物体
    /// </summary>
    public void Disable3DScene()
    {
        GameObject.Find("#SCENE#").SetActive(false);
        GameObject.Find("#CAMERA#").SetActive(false);
    }
}