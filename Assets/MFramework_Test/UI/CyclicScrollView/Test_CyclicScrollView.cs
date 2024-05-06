using UnityEngine;
using MFramework;
using System.Collections.Generic;

public class Test_CyclicScrollView : CyclicScrollView<Cell, Data>
{
    public List<Data> dataTemplates;

    private List<Data> dataList;

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
        Init(dataList);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.E))
        {
            int random = Random.Range(0, dataList.Count);
            dataList.RemoveAt(random);
            dataList.RemoveAt(random);
            dataList.RemoveAt(random);
            dataList.RemoveAt(random);

            Refresh();
        }
    }
}