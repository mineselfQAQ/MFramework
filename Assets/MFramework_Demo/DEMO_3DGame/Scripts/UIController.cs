using MFramework;
using System.Collections.Generic;

public class UIController : ComponentSingleton<UIController>
{
    public UIRoot bottomRoot;
    public UIRoot topRoot;

    public Dictionary<string, UIPanel> panelDic;

    public static readonly string panelPrepath = "Assets/MFramework_Demo/DEMO_3DGame/MFrameworkUI/Panels";
    public static readonly string widgetPrepath = "Assets/MFramework_Demo/DEMO_3DGame/MFrameworkUI/Widgets";

    protected override void Awake()
    {
        base.Awake();
        UIManager.Instance.SetDontDestroyOnLoad();

        panelDic = new Dictionary<string, UIPanel>();

        bottomRoot = UIManager.Instance.CreateRoot("BOTTOMROOT", 0, 999);
        topRoot = UIManager.Instance.CreateRoot("TOPROOT", 1000, 1999);

        //用于切换场景过渡用
        CreatePanel<LoadingPanel>(topRoot, "Loading", $"{panelPrepath}/LoadingPanel/LoadingPanel.prefab", false);
        
        CreatePanel<TitleScreenPanel>(bottomRoot, "TitleScreen", $"{panelPrepath}/TitleScreenPanel/TitleScreenPanel.prefab", true);
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

    public void TitleScreenToFileSelect()
    {
        bottomRoot.ClosePanel("TitleScreen");

        if (!panelDic.ContainsKey("FileSelect"))
        {
            FileSelectPanel fileSelect = (FileSelectPanel)CreatePanel<FileSelectPanel>(bottomRoot, "FileSelect", $"{panelPrepath}/FileSelect/FileSelectPanel.prefab", true);
            fileSelect.Init();
        }
        else
        {
            bottomRoot.OpenPanel("FileSelect");
        }
    }

    public void FileSelectToLevelSelect()
    {
        bottomRoot.ClosePanel("FileSelect");


        if (!panelDic.ContainsKey("LevelSelect"))
        {
            LevelSelectPanel levelSelect = (LevelSelectPanel)CreatePanel<LevelSelectPanel>(bottomRoot, "LevelSelect", $"{panelPrepath}/LevelSelect/LevelSelectPanel.prefab", true);
            levelSelect.Init();
        }
        else
        {
            bottomRoot.OpenPanel("LevelSelect");
        }
    }

    public void LevelSelectBackToFileSelect()
    {
        bottomRoot.ClosePanel("LevelSelect");
        bottomRoot.OpenPanel("FileSelect");
    }
}