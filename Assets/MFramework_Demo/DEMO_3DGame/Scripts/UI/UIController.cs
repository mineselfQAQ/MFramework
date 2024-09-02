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

        //用于切换场景过渡用
        flashEffect = (FlashEffect)CreatePanel<FlashEffect>(topRoot, flashEffectName, $"{panelPrepath}/FlashEffect/FlashEffect.prefab", false);
        topRoot.SetSortingOrder(flashEffectName, 1995);
        CreatePanel<LoadingPanel>(topRoot, loadPanelName, $"{panelPrepath}/LoadingPanel/LoadingPanel.prefab", false);
        topRoot.SetSortingOrder(loadPanelName, 1996);
        CreatePanel<PausePanel>(topRoot, pausePanelName, $"{panelPrepath}/PausePanel/PausePanel.prefab", false);
        topRoot.SetSortingOrder(pausePanelName, 1997);
        CreatePanel<TransitionPanel>(topRoot, transitionPanelName, $"{panelPrepath}/TransitionPanel/TransitionPanel.prefab", false);
        topRoot.SetSortingOrder(transitionPanelName, 1998);
        CreatePanel<SponsorDisplayPanel>(topRoot, sponsorDisplayPanelName, $"{panelPrepath}/SponsorDisplayPanel/SponsorDisplayPanel.prefab", false);
        topRoot.SetSortingOrder(sponsorDisplayPanelName, 1999);

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

    public void OpenTakeBloodRestart(Action onFinish = null)
    {
        topRoot.OpenPanel(transitionPanelName);
        topRoot.GetPanel<TransitionPanel>(transitionPanelName).OpenTakeBloodRestartWidget(onFinish);
    }
    public void CloseTakeBloodRestart(Action onFinish = null)
    {
        topRoot.GetPanel<TransitionPanel>(transitionPanelName).CloseTakeBloodRestartWidget(() =>
        {
            topRoot.ClosePanel(transitionPanelName, onFinish);
        });
    }

    public void OpenTransitionPanel(Action onFinish = null)
    {
        topRoot.OpenPanel(transitionPanelName, onFinish);
    }
    public void CloseTransitionPanel(Action onFinish = null)
    {
        topRoot.ClosePanel(transitionPanelName, onFinish);
    }

    public void OpenSponsorDisplayPanel(Action onFinish = null)
    {
        topRoot.OpenPanel(sponsorDisplayPanelName, onFinish);
    }
    public void CloseSponsorDisplayPanel(Action onFinish = null)
    {
        topRoot.ClosePanel(sponsorDisplayPanelName, onFinish);
    }

    public void OpenTitleScreenPanel(Action onFinish = null)
    {
        bottomRoot.OpenPanel(titleScreenPanelName, onFinish);
    }

    public void OpenPausePanel(Action onFinish = null)
    {
        topRoot.OpenPanel(pausePanelName, onFinish);
    }
    public void ClosePausePanel(Action onFinish = null)
    {
        topRoot.ClosePanel(pausePanelName, onFinish);
    }

    public void CreateOrOpenHUD(Action onFinish = null)
    {
        if (!topRoot.ExistPanel(HUDPanelName))
        {
            CreatePanel<HUDPanel>(topRoot, HUDPanelName, $"{panelPrepath}/HUDPanel/HUDPanel.prefab", true);
        }
        else
        {
            topRoot.OpenPanel(HUDPanelName, onFinish);
        }
    }
    public void CloseHUD(Action onFinish = null)
    {
        topRoot.ClosePanel(HUDPanelName, onFinish);
    }
    public void RefreshHUD()
    {
        topRoot.GetPanel<HUDPanel>(HUDPanelName).ResetPanel();
    }

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

    public void TriggerFlash()
    {
        topRoot.OpenPanel(flashEffectName);

        flashEffect.Flash(flashDuration, () =>
        {
            topRoot.ClosePanel(flashEffectName);
        });
    }

    /// <summary>
    /// 不再显示场景中的3D物体
    /// </summary>
    public void Disable3DScene()
    {
        GameObject.Find("#SCENE#").SetActive(false);
        GameObject.Find("#CAMERA#").SetActive(false);
    }
}