using UnityEngine;
using MFramework;
using System.Collections.Generic;

public class Test_CyclicScrollView : CyclicScrollView<Cell, Data>
{
    public List<Data> dataTemplates;

    private List<Data> dataList1;
    private Data[] testDatas;

    protected override void ResetCellData(Cell cell, Data data, int dataIndex)
    {
        cell.gameObject.SetActive(true);
        cell.UpdateDisplay(data.iconSprite, data.name);
    }

    private void Start()
    {
        dataList1 = new List<Data>();
        testDatas = new Data[128];
        for (int i = 0; i < testDatas.Length; i++)
        {
            int value = Random.Range(0, 3);//[0,2]
            testDatas[i] = dataTemplates[value];
        }
        Init(testDatas);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.E))
        {
            content.anchoredPosition = Vector2.zero;
        }
    }
}