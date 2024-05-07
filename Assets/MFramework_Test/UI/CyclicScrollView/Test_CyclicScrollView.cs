using UnityEngine;
using MFramework;
using System.Collections.Generic;

public class Test_CyclicScrollView : CyclicScrollView<Cell, Data>
{
    public List<Data> dataTemplates;//Cell需要的数据预设

    private List<Data> dataList;
    private List<Data> dataList2;

    protected override void ResetCellData(Cell cell, Data data, int dataIndex)
    {
        cell.gameObject.SetActive(true);
        cell.UpdateDisplay(data.iconSprite, data.name);
    }

    private void Start()
    {
        dataList = new List<Data>();
        for (int i = 0; i < 100; i++)
        {
            int value = Random.Range(0, 3);//[0,2]
            dataList.Add(dataTemplates[value]);
        }
        dataList2 = new List<Data>();
        for (int i = 0; i < 10; i++)
        {
            int value = Random.Range(0, 3);//[0,2]
            dataList2.Add(dataTemplates[value]);
        }

        Init(dataList);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.E))
        {
            int random = Random.Range(0, dataList.Count);
            dataList.RemoveAt(random);

            Refresh();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            int random = Random.Range(0, dataList.Count);
            dataList.Insert(random, dataTemplates[Random.Range(0, 3)]);

            Refresh();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Refresh(dataList2);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            MoveTo(0);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            MoveToBundle(2);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            MoveToCell(8);
        }
    }
}