using MFramework.DLC;
using MFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        graph.AddEdge(new Edge<int>(2, 3));

        graph.RemoveVertex(3);

        MLog.Print(graph.VertexCount);
        MLog.Print(graph.EdgeCount);
    }
}
