using UnityEngine;
using MFramework.DLC;
using static MFramework.DLC.MUndirGraphAdjList;
using MFramework;
using System.Text;

public class Test_MUndirGraphAdjList : MonoBehaviour
{
    private void Start()
    {
        //无权图(默认设置为1)  Tip：也可以有权，只是暂时是无权的
        MUndirGraphAdjList unweightGraph = new MUndirGraphAdjList(
            //提供边，能推算出有哪些顶点
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

        //有权图(默认设置为1)
        MUndirGraphAdjList weightGraph = new MUndirGraphAdjList(
            //提供边，能推算出有哪些顶点
            new Edge[]
            {
                new Edge(0, 2, 0.1f),
                new Edge(0, 4, 0.5f),
                new Edge(1, 4, 1.0f),
                new Edge(2, 3, 0.5f),
                new Edge(3, 4, 0.1f)
            });
        weightGraph.Print();

        unweightGraph.AddVertex(8);//孤立节点
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
