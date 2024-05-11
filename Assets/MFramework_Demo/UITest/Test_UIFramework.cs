using MFramework;
using UnityEngine;

public class Test_UIFramework : MonoBehaviour
{
    private UIRoot root;

    private void Start()
    {
        root = UIManager.Instance.CreateRoot("ROOT", 0, 999);

        TestPanel panel = root.CreatePanel<TestPanel>(@"Assets\MFramework_Demo\UITest\TestPanel.prefab");
        TestPanel panel2 = root.CreatePanel<TestPanel>("TestPanel2", @"Assets\MFramework_Demo\UITest\TestPanel.prefab");
        TestPanel panel3 = root.CreatePanel<TestPanel>("TestPanel3", @"Assets\MFramework_Demo\UITest\TestPanel.prefab");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            root.DestroyPanel("TestPanel");
        }
    }
}