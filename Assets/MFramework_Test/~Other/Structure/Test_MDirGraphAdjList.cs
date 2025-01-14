using MFramework;
using MFramework.DLC;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static MFramework.DLC.MDirGraphAdjList;

public class Test_MDirGraphAdjList : MonoBehaviour
{
    private void Start()
    {
        //��Ȩͼ(Ĭ������Ϊ1)  Tip��Ҳ������Ȩ��ֻ����ʱ����Ȩ��
        MDirGraphAdjList unweightGraph = new MDirGraphAdjList(
            //�ṩ�ߣ������������Щ����
            new Edge[]
            {
                new Edge(0, 2),
                new Edge(0, 4),
                new Edge(1, 4),
                new Edge(2, 3),
                new Edge(3, 4)
            });
        unweightGraph.Print();

        //��Ȩͼ(Ĭ������Ϊ1)
        MDirGraphAdjList weightGraph = new MDirGraphAdjList(
            //�ṩ�ߣ������������Щ����
            new Edge[]
            {
                new Edge(0, 2, 0.1f),
                new Edge(0, 4, 0.5f),
                new Edge(1, 4, 1.0f),
                new Edge(2, 3, 0.5f),
                new Edge(3, 4, 0.1f)
            });
        weightGraph.Print();

        //������
        MDirGraphAdjList cycleGraph = new MDirGraphAdjList(
            //�ṩ�ߣ������������Щ����
            new Edge[]
            {
                //��1��0->1->2->0
                //��2��0->1->3->4->2->0
                new Edge(0, 1),
                new Edge(1, 2),
                new Edge(2, 0),
                new Edge(1, 3),
                new Edge(3, 4),
                new Edge(4, 2),
            });

        StringBuilder sb = new StringBuilder();
        sb.Append('\n');
        List<List<int>> lists = cycleGraph.FindCycle();
        foreach (var list in lists)
        {
            foreach (var i in list)
            {
                sb.Append(i.ToString().PadRight(4));
            }
            sb.Append('\n');
        }
        MLog.Print(sb.ToString());
    }
}
