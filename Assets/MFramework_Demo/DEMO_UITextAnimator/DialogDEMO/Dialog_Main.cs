using MFramework;
using UnityEngine;

public class Dialog_Main : MonoBehaviour
{
    public string playerName;
    public Sprite profilePhoto;

    private UIRoot root;
    private DEMODialogPanel dialogPanel;

    private void Start()
    {
        root = UIManager.Instance.CreateRoot("Root", 0, 999);
        dialogPanel = root.CreatePanel<DEMODialogPanel>(@"Assets/MFramework_Demo/DEMO_UITextAnimator/DialogDEMO/DialogPanel/DEMODialogPanel.prefab", true);
        dialogPanel.Init(playerName, profilePhoto);
    }

    private void Update()
    {
        dialogPanel.Update();
    }
}
