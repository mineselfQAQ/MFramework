using MFramework;
using UnityEngine;

public class Dialog_Main : MonoBehaviour
{
    public string playerName;
    public Sprite profilePhoto;

    private UIRoot root;
    private DialogPanel dialogPanel;

    private void Start()
    {
        root = UIManager.Instance.CreateRoot("Root", 0, 999);
        dialogPanel = root.CreatePanel<DialogPanel>(@"Assets/MFramework_Demo/DEMO_UITextAnimator/DialogDEMO/DialogPanel/DialogPanel.prefab", true);
        dialogPanel.Init(playerName, profilePhoto);
    }

    private void Update()
    {
        dialogPanel.Update();
    }
}
