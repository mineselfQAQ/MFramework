using MFramework;
using UnityEngine;

public class Test_UIFramework : MonoBehaviour
{
    private UIRoot root;

    private void Start()
    {
        root = UIManager.Instance.CreateRoot("ROOT", 0, 999);

        TestPanel panel = root.CreatePanel<TestPanel>(@"F:\MineselfDemo\MFramework\Assets\MFramework_Demo\UITest\TestPanel.prefab");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            root.DestroyPanel("TestPanel");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            root.SetPanelVisible<TestPanel>(false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            root.SetPanelVisible<TestPanel>(true);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            root.ClosePanel<TestPanel>();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            root.OpenPanel<TestPanel>();
        }
    }
}