using MFramework;
using MFramework.DLC;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Test_MUndirectedGraph : MonoBehaviour
{
    private void Start()
    {
        MUndirectedGraph<int, UndirectedEdge<int>> graph = new();

        graph.AddVertex(1);
        graph.AddVertex(2);
        graph.AddVertex(3);
        graph.AddVertex(4);
        graph.AddVertex(5);

        graph.AddEdge(new UndirectedEdge<int>(1, 2));
        graph.AddEdge(new UndirectedEdge<int>(1, 3));
        graph.AddEdge(new UndirectedEdge<int>(1, 4));
        graph.AddEdge(new UndirectedEdge<int>(2, 3));
        graph.AddEdge(new UndirectedEdge<int>(4, 1));
        graph.AddEdge(new UndirectedEdge<int>(3, 4));
        graph.AddEdge(new UndirectedEdge<int>(4, 5));
        graph.AddEdge(new UndirectedEdge<int>(4, 5));

        //孤岛
        graph.AddVertex(6);
        graph.AddVertex(7);
        graph.AddEdge(new UndirectedEdge<int>(6, 7));
        graph.AddEdge(new UndirectedEdge<int>(7, 6));//不存在平行边，等于没添加

        var edges = graph.GetAdjacentEdges(1);
        foreach (var edge in edges)
        {
            MLog.Print(edge);
        }

        var edge1 = graph.GetEdge(1, 3);
        MLog.Print(edge1);
        edge1 = graph.GetEdge(3, 1);
        MLog.Print(edge1);

        StringBuilder sb = new StringBuilder();
        foreach (var val in graph.BFS())
        {
            sb.Append($"{val} ");
        }
        MLog.Print(sb);

        sb = new StringBuilder();
        foreach (var val in graph.DFS())
        {
            sb.Append($"{val} ");
        }
        MLog.Print(sb);

        MLog.Print(graph.HasCycle());

        sb = new StringBuilder();
        List<List<int>> cycles = graph.FindCycle();
        if (cycles.Count > 0) sb.Append($"有环：\n");
        foreach (var cycle in cycles)
        {
            foreach (var val in cycle)
            {
                sb.Append($"{val} ");
            }
            sb.Append('\n');
        }
        MLog.Print(sb);
    }
}
