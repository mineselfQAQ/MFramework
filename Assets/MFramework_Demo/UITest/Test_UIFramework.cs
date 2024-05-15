using MFramework;
using UnityEngine;

public class Test_UIFramework : MonoBehaviour
{
    private UIRoot bottomRoot;
    private UIRoot topRoot;

    private void Start()
    {
        bottomRoot = UIManager.Instance.CreateRoot("Bottom", 0, 999);
        topRoot = UIManager.Instance.CreateRoot("Top", 1000, 1200);

        //TestPanel panel = root.CreatePanel<TestPanel>(@"Assets\MFramework_Demo\UITest\TestPanel.prefab");

        EmptyPanel panel1 = bottomRoot.CreatePanel<EmptyPanel>("Background", @"Assets\MFramework_Demo\UITest\Canvas.prefab");
        EmptyPanel panel2 = topRoot.CreatePanel<EmptyPanel>("Bag1", @"Assets\MFramework_Demo\UITest\Canvas_1.prefab");
        EmptyPanel panel3 = topRoot.CreatePanel<EmptyPanel>("Bag2", @"Assets\MFramework_Demo\UITest\Canvas_2.prefab");
        EmptyPanel panel4 = topRoot.CreatePanel<EmptyPanel>("Bag3", @"Assets\MFramework_Demo\UITest\Canvas_3.prefab");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            topRoot.DestroyPanel("Bag3");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            topRoot.SetPanelVisible("Bag1", false, true);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            topRoot.SetPanelVisible("Bag1", true, true);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            topRoot.ClosePanel("Bag2");
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            topRoot.OpenPanel("Bag2", null, true);
        }
    }
}