using UnityEngine;
using MFramework.DLC;
using static MFramework.DLC.MUndirGraphAdjList;
using MFramework;
using System.Text;
using System.Collections.Generic;

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

        //环测试
        MUndirGraphAdjList cycleGraph = new MUndirGraphAdjList(
            //提供边，能推算出有哪些顶点
            new Edge[]
            {
                //环1：0->1->2->0
                //环2：0->1->3->4->2->0
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
