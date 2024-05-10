using MFramework;
using UnityEngine;

public class Test_UIFramework : MonoBehaviour
{
    private UIRoot root;

    private void Start()
    {
        root = UIManager.Instance.CreateRoot("ROOT", 0, 999);

        TestPanel panel = root.CreatePanel<TestPanel>
            ("TestPanel", @"Assets\MFramework_Demo\UITest\TestPanel.prefab", 1);
    }
}