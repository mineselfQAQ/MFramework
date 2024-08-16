using MFramework;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class UIController : ComponentSingleton<UIController>
{
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

    public static readonly string loadPanelName = "LOADING";
    public static readonly string pausePanelName = "PAUSE";
    public static readonly string HUDPanelName = "HUD";
    public static readonly string titleScreenPanelName = "TITLESCREEN";
    public static readonly string fileSelectPanelName = "FILESELECT";
    public static readonly string levelSelectPanelName = "LEVELSELECT";

    public static readonly string flashEffectName = "FLASHEFFECT";

    public static readonly string titleScreenSceneName = "3DGame_TitleScreen";

    protected FlashEffect flashEffect;

    protected override void Awake()
    {
        base.Awake();
        //UIManager.Instance.SetDontDestroyOnLoad();

        panelDic = new Dictionary<string, UIPanel>();

        bottomRoot = UIManager.Instance.CreateRoot("BOTTOMROOT", 0, 999);
        topRoot = UIManager.Instance.CreateRoot("TOPROOT", 1000, 1999);


        //�����л�����������
        flashEffect = (FlashEffect)CreatePanel<FlashEffect>(topRoot, flashEffectName, $"{panelPrepath}/FlashEffect/FlashEffect.prefab", false);
        topRoot.SetSortingOrder(flashEffectName, 1997);
        CreatePanel<LoadingPanel>(topRoot, loadPanelName, $"{panelPrepath}/LoadingPanel/LoadingPanel.prefab", false);
        topRoot.SetSortingOrder(loadPanelName, 1998);
        CreatePanel<PausePanel>(topRoot, pausePanelName, $"{panelPrepath}/PausePanel/PausePanel.prefab", false);
        topRoot.SetSortingOrder(pausePanelName, 1999);

        //��������ֱ������(���ڲ���)������TitleScreenPanel
#if UNITY_EDITOR
        if (EditorSceneManager.GetActiveScene().name != titleScreenSceneName)
        {
            return;
        }
#endif

        //��ʼ����TitleScreenPanel
        CreatePanel<TitleScreenPanel>(bottomRoot, titleScreenPanelName, $"{panelPrepath}/TitleScreenPanel/TitleScreenPanel.prefab", true);
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

    public void OpenPausePanel()
    {
        topRoot.OpenPanel(pausePanelName);
    }
    public void ClosePausePanel()
    {
        topRoot.ClosePanel(pausePanelName);
    }

    public void CreateHUD()
    {
        CreatePanel<HUDPanel>(topRoot, HUDPanelName, $"{panelPrepath}/HUDPanel/HUDPanel.prefab", true);
    }
    public void DestroyHUD()
    {
        DestroyPanel(topRoot, HUDPanelName);
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
}