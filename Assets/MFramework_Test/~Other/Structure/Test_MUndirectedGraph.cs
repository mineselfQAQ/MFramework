using MFramework;
using MFramework.DLC;
using System.Collections.Generic;
using UnityEngine;

public class Test_MUndirectedGraph : MonoBehaviour
{
    private void Start()
    {
        MUndirectedGraph<int, UndirectedEdge<int>> graph = new();

        graph.AddVertex(1);
        graph.AddVertex(2);
        graph.AddVertex(3);

        graph.AddEdge(new UndirectedEdge<int>(1, 2));
        graph.AddEdge(new UndirectedEdge<int>(1, 3));
        graph.AddEdge(new UndirectedEdge<int>(2, 3));

        var edges = graph.GetAdjacentEdges(1);
        foreach (var edge in edges)
        {
            MLog.Print(edge);
        }

        var edge1 = graph.GetEdge(1, 3);
        MLog.Print(edge1);
        edge1 = graph.GetEdge(3, 1);
        MLog.Print(edge1);
    }
}
