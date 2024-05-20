using MFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Main : MonoBehaviour
{
    public Sprite background;
    public string title;

    private UIRoot root;

    private TitleMenu titleMenu;

    private void Start()
    {
        root = UIManager.Instance.CreateRoot("Root", 1, 999);

        titleMenu = root.CreatePanel<TitleMenu>(@"Assets\MFramework_Demo\UIPanelDEMO\2_CommonMenu\Prefab\TitleMenu.prefab");
        titleMenu.Init(background, title);
        titleMenu.Open();
    }

    private void Update()
    {
       // Debug.Log(titleMenu.AnimState);
    }
}
