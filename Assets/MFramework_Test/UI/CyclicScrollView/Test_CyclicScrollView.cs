using UnityEngine;
using MFramework;
using System.Collections.Generic;

public class Test_CyclicScrollView : CyclicScrollView<Cell, Data>
{
    public List<Data> dataTemplates;

    private List<Data> dataList1;
    private Data[] datas;

    protected override void ResetCellData(Cell cell, Data data, int dataIndex)
    {
        cell.gameObject.SetActive(true);
        cell.UpdateDisplay(data.iconSprite, data.name);
    }

    private void Start()
    {
        dataList1 = new List<Data>();
        datas = new Data[100];
        for (int i = 0; i < datas.Length; i++)
        {
            int value = Random.Range(0, 3);//[0,2]
            datas[i] = dataTemplates[value];
        }
        Init(datas);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            content.anchoredPosition = Vector2.zero;
        }
    }
}