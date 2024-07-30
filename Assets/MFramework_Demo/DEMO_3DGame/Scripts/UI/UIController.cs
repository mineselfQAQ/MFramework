using MFramework;
using System.Collections.Generic;

public class UIController : ComponentSingleton<UIController>
{
    public UIRoot bottomRoot;
    public UIRoot topRoot;

    public Dictionary<string, UIPanel> panelDic;

    public static readonly string panelPrepath = "Assets/MFramework_Demo/DEMO_3DGame/MFrameworkUI/Panels";
    public static readonly string widgetPrepath = "Assets/MFramework_Demo/DEMO_3DGame/MFrameworkUI/Widgets";

    public static readonly string loadPanelName = "LOADING";
    public static readonly string pausePanelName = "PAUSE";
    public static readonly string titleScreenPanelName = "TITLESCREEN";
    public static readonly string fileSelectPanelName = "FILESELECT";
    public static readonly string levelSelectPanelName = "LEVELSELECT";

    protected override void Awake()
    {
        base.Awake();
        UIManager.Instance.SetDontDestroyOnLoad();

        panelDic = new Dictionary<string, UIPanel>();

        bottomRoot = UIManager.Instance.CreateRoot("BOTTOMROOT", 0, 999);
        topRoot = UIManager.Instance.CreateRoot("TOPROOT", 1000, 1999);

        //用于切换场景过渡用
        CreatePanel<LoadingPanel>(topRoot, loadPanelName, $"{panelPrepath}/LoadingPanel/LoadingPanel.prefab", false);
        CreatePanel<PausePanel>(topRoot, pausePanelName, $"{panelPrepath}/PausePanel/PausePanel.prefab", false);

        CreatePanel<TitleScreenPanel>(bottomRoot, titleScreenPanelName, $"{panelPrepath}/TitleScreenPanel/TitleScreenPanel.prefab", true);
    }

    private void Update()
    {
        
    }

    public UIPanel CreatePanel<T>(UIRoot root, string id, string prefabPath, bool autoEnter) where T : UIPanel
    {
        UIPanel panel = root.CreatePanel<T>(id, prefabPath, autoEnter);
        panelDic.Add(id, panel);

        return panel;
    }

    public void OpenPausePanel()
    {
        topRoot.OpenPanel(pausePanelName);
    }
    public void ClosePausePanel()
    {
        topRoot.ClosePanel(pausePanelName);
    }

    public void TitleScreenToFileSelect()
    {
        bottomRoot.ClosePanel(titleScreenPanelName);

        if (!panelDic.ContainsKey(fileSelectPanelName))
        {
            FileSelectPanel fileSelect = (FileSelectPanel)CreatePanel<FileSelectPanel>(bottomRoot, fileSelectPanelName, $"{panelPrepath}/FileSelectPanel/FileSelectPanel.prefab", true);
            fileSelect.Init();
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
            LevelSelectPanel levelSelect = (LevelSelectPanel)CreatePanel<LevelSelectPanel>(bottomRoot, levelSelectPanelName, $"{panelPrepath}/LevelSelectPanel/LevelSelectPanel.prefab", true);
            levelSelect.Init();
        }
        else
        {
            bottomRoot.OpenPanel(levelSelectPanelName);
        }
    }

    public void LevelSelectBackToFileSelect()
    {
        bottomRoot.ClosePanel(levelSelectPanelName);
        bottomRoot.OpenPanel(fileSelectPanelName);
    }
}