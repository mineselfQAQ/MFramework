using MFramework;
using UnityEngine;

public class Main : MonoBehaviour
{
    public Sprite background;
    public string titleName;

    private UIRoot root;

    private void Start()
    {
        root = UIManager.Instance.CreateRoot("Root", 0, 999);
        var startPanel = root.CreatePanel<StartPanel>(@"Assets/MFramework_Demo/DEMO_2DGame/StartMenu/Panel_StartPanel/StartPanel.prefab", true);
        startPanel.Init(background, titleName);
    }
}