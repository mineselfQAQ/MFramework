using MFramework;
using MFramework.DLC;
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

        unweightGraph.AddVertex(8);//�����ڵ�
        var list = unweightGraph.BFS();
        StringBuilder sb = new StringBuilder();
        foreach (var i in list)
        {
            sb.Append($"{i} ");
        }
        MLog.Print(sb);
        list = unweightGraph.DFS();
        sb = new StringBuilder();
        foreach (var i in list)
        {
            sb.Append($"{i} ");
        }
        MLog.Print(sb);
    }
}
