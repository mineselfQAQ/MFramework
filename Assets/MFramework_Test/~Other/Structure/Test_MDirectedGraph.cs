using MFramework.DLC;
using MFramework;
using UnityEngine;
using System.Text;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Collections.Generic;

public class Test_MDirectedGraph : MonoBehaviour
{
    private void Start()
    {
        MDirectedGraph<int, Edge<int>> graph = new();

        graph.AddVertex(1);
        graph.AddVertex(2);
        graph.AddVertex(3);
        graph.AddVertex(4);
        graph.AddVertex(5);

        graph.AddEdge(new Edge<int>(1, 2));
        graph.AddEdge(new Edge<int>(1, 3));
        graph.AddEdge(new Edge<int>(1, 4));
        graph.AddEdge(new Edge<int>(2, 3));
        graph.AddEdge(new Edge<int>(4, 1));
        graph.AddEdge(new Edge<int>(3, 4));
        graph.AddEdge(new Edge<int>(4, 5));

        //graph.RemoveVertex(3);

        MLog.Print(graph.VertexCount);
        MLog.Print(graph.EdgeCount);

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
        if(cycles.Count > 0) sb.Append($"ÓÐ»·£º\n");
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
