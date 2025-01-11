using MFramework;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using UnityEngine;
using UnityEngine.UI;

public delegate int Fun1(int a);

public class Test11 : MonoBehaviour
{
    public static readonly string panelPrepath = "Assets/MFramework_Demo/DEMO_3DGame/MFrameworkUI/Panels";

    void Start()
    {
        AssetBundle ab = MABUtility.LoadAB("D:/___UnityProject___/MFramework/PlatformGame_AssetBundle/WINDOWS_ENCRYPT/init.ab");
        var prefab = ab.LoadAsset<GameObject>("InitPanel");
        Instantiate(prefab);

        UIRoot root = new UIRoot("A", 0, 999);
        UIPanel panel = root.CreatePanel<InitPanel>("ABC", $"{panelPrepath}/InitPanel/InitPanel.prefab", true);
        panel.Init();
    }

    void Update()
    {

    }
}