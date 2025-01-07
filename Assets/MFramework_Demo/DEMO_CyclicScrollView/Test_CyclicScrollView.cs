using UnityEngine;
using MFramework;
using System.Collections.Generic;

public class Test_CyclicScrollView : MScrollView<Cell, Data>
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

        Init(dataList2);
    }

    protected override void Update()
    {
        base.Update();

        //删除元素
        if (Input.GetKeyDown(KeyCode.E))
        {
            int random = Random.Range(0, dataList2.Count);
            dataList2.RemoveAt(random);

            Refresh();
        }
        //添加元素
        if (Input.GetKeyDown(KeyCode.R))
        {
            int random = Random.Range(0, dataList2.Count);
            dataList2.Insert(random, dataTemplates[Random.Range(0, 3)]);

            Refresh();
            MoveTo(1);
        }

        //使用另一个Data数组进行刷新
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Refresh(dataList2);
        }

        //移动至顶部
        if (Input.GetKeyDown(KeyCode.Z))
        {
            MoveTo(0);
        }
        //移动至第2行
        if (Input.GetKeyDown(KeyCode.X))
        {
            MoveToBundle(2);
        }
        //移动至第8个元素所在行
        if (Input.GetKeyDown(KeyCode.C))
        {
            MoveToCell(8);
        }
    }
}