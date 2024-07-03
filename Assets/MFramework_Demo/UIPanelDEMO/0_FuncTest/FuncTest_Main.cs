using MFramework;
using UnityEngine;

public class FuncTest_Main : MonoBehaviour
{
    private UIRoot root;

    private void Start()
    {
        root = UIManager.Instance.CreateRoot("Root", 0, 999);

        var panel = root.CreatePanel<FuncTest_Panel>("Panel", @"Assets\MFramework_Demo\UIPanelDEMO\0_FuncTest\Prefab\FuncTest_Panel.prefab", true);
        panel.Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var panel = root.GetPanel<FuncTest_Panel>("Panel");
            panel.DestroyAllWidgets();
        }
    }
}
