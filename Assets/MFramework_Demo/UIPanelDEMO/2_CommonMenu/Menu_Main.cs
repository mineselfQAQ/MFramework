using MFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Main : MonoBehaviour
{
    public Sprite background;
    public string title;

    private UIRoot root;

    private void Start()
    {
        root = UIManager.Instance.CreateRoot("Root", 1, 999);

        var titleMenu = root.CreatePanel<TitleMenu>(@"Assets\MFramework_Demo\UIPanelDEMO\2_CommonMenu\Prefab\TitleMenu.prefab");
        titleMenu.Init(background, title);
    }
}
