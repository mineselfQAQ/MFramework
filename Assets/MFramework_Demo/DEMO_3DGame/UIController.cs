using MFramework;
using System.Collections.Generic;

public class UIController : ComponentSingleton<UIController>
{
    public UIRoot bottomRoot;
    public UIRoot topRoot;

    public Dictionary<string, UIPanel> panelDic;

    protected override void Awake()
    {
        base.Awake();
        UIManager.Instance.SetDontDestroyOnLoad();

        panelDic = new Dictionary<string, UIPanel>();

        bottomRoot = UIManager.Instance.CreateRoot("BOTTOMROOT", 0, 999);
        topRoot = UIManager.Instance.CreateRoot("TOPROOT", 1000, 1999);

        //用于切换场景过渡用
        CreatePanel<LoadingPanel>(topRoot, "Loading", "Assets/MFramework_Demo/DEMO_3DGame/Panels/LoadingPanel/LoadingPanel.prefab", false);
        
        CreatePanel<TitleScreenPanel>(bottomRoot, "TitleScreen", "Assets/MFramework_Demo/DEMO_3DGame/Panels/TitleScreenPanel/TitleScreenPanel.prefab", true);
    }

    private void Update()
    {
        
    }

    public void CreatePanel<T>(UIRoot root, string id, string prefabPath, bool autoEnter) where T : UIPanel
    {
        UIPanel panel = root.CreatePanel<T>(id, prefabPath, autoEnter);
        panelDic.Add(id, panel);
    }

    public void TitleScreenToFileSelect()
    {
        bottomRoot.ClosePanel("TitleScreen");

        if (!panelDic.ContainsKey("FileSelect"))
        {
            CreatePanel<FileSelectPanel>(bottomRoot, "FileSelect", "Assets/MFramework_Demo/DEMO_3DGame/Panels/FileSelect/FileSelectPanel.prefab", true);
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
            CreatePanel<LevelSelectPanel>(bottomRoot, "LevelSelect", "Assets/MFramework_Demo/DEMO_3DGame/Panels/LevelSelect/LevelSelectPanel.prefab", true);
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