using MFramework;
using System.Collections.Generic;
using UnityEngine;

public class Bag_Main : MonoBehaviour
{
    private UIRoot backgroundRoot;
    private UIRoot bagRoot;

    private List<Bag> bagList;

    private void Start()
    {
        backgroundRoot = UIManager.Instance.CreateRoot("Background", 0, 999);
        bagRoot = UIManager.Instance.CreateRoot("Bag", 1000, 1200);

        EmptyPanel background = backgroundRoot.CreatePanel<EmptyPanel>("Background", @"Assets\MFramework_Demo\UIPanelDEMO\1_Bag\Prefab\Background.prefab");

        bagList = new List<Bag>();
        Color[] colors = new Color[3] {
            new Color(0.25f, 0.72f, 0.6f, 1f), 
            new Color(0.4f, 0.4f, 0.8f, 1f), 
            new Color(0.75f, 0.54f, 0.54f, 1f) 
        };
        Vector2[] sizes = new Vector2[3] { 
            new Vector2(500, 800), 
            new Vector2(500, 800), 
            new Vector2(800, 500) 
        };
        for (int i = 0; i < 3; i++)
        {
            Bag bag = bagRoot.CreatePanel<Bag>($"Bag{i + 1}", @"Assets\MFramework_Demo\UIPanelDEMO\1_Bag\Prefab\Bag.prefab");
            var drager = DragEventListener.GetOrAdd(bag.gameObject);
            drager.onDrag += (eventData) => { bag.rectTransform.anchoredPosition += eventData.delta; };
            bag.Init(i + 1, colors[i], sizes[i]);
            bagList.Add(bag);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (bagList[0].showState == UIPanelShowState.On)
            {
                //bagRoot.SetPanelVisible("Bag1", false, true);
                bagRoot.ClosePanel("Bag1");
            }
            else
            {
                //bagRoot.SetPanelVisible("Bag1", true, true);
                bagRoot.OpenPanel("Bag1", true);
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (bagList[1].showState == UIPanelShowState.On)
            {
                //bagRoot.SetPanelVisible("Bag2", false, true);
                bagRoot.ClosePanel("Bag2");
            }
            else
            {
                //bagRoot.SetPanelVisible("Bag2", true, true);
                bagRoot.OpenPanel("Bag2", true);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (bagList[2].showState == UIPanelShowState.On)
            {
                //bagRoot.SetPanelVisible("Bag3", false, true);
                bagRoot.ClosePanel("Bag3");
            }
            else
            {
                //bagRoot.SetPanelVisible("Bag3", true, true);
                bagRoot.OpenPanel("Bag3", true);
            }
        }
    }
}