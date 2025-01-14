using UnityEngine;
using MFramework.DLC;
using static MFramework.DLC.MUndirGraphAdjList;
using MFramework;
using System.Text;

public class Test_MUndirGraphAdjList : MonoBehaviour
{
    private void Start()
    {
        //��Ȩͼ(Ĭ������Ϊ1)  Tip��Ҳ������Ȩ��ֻ����ʱ����Ȩ��
        MUndirGraphAdjList unweightGraph = new MUndirGraphAdjList(
            //�ṩ�ߣ������������Щ����
            new Edge[]
            {
                new Edge(0, 2),
                new Edge(0, 4),
                new Edge(1, 4),
                new Edge(2, 3),
                new Edge(3, 4)
            });
        //unweightGraph.RemoveEdge(2, 3);
        unweightGraph.Print();

        //��Ȩͼ(Ĭ������Ϊ1)
        MUndirGraphAdjList weightGraph = new MUndirGraphAdjList(
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
