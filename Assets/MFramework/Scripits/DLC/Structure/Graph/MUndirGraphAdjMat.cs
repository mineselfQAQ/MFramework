using System;
using System.Collections.Generic;
using System.Text;

namespace MFramework.DLC
{
    /// <summary>
    /// 无向有权邻接矩阵图
    /// </summary>
    public class MUndirGraphAdjMat
    {
        public struct Edge
        {
            public int vertex1;
            public int vertex2;
            public float weight;

            public Edge(int vertex1, int vertex2, float weight = 1.0f)
            {
                this.vertex1 = vertex1;
                this.vertex2 = vertex2;
                this.weight = weight;
            }
        }

        internal List<int> vertices;//顶点与对应id
        internal List<List<float>> adjMat;//邻接矩阵

        public MUndirGraphAdjMat()
        {
            this.vertices = new List<int>();
            this.adjMat = new List<List<float>>();
        }
        public MUndirGraphAdjMat(int[] vertices, Edge[] edges)
        {
            this.vertices = new List<int>();
            this.adjMat = new List<List<float>>();

            //添加顶点
            foreach (int val in vertices)
            {
                AddVertex(val);
            }

            //添加边
            foreach (var e in edges)
            {
                AddEdge(e.vertex1, e.vertex2, e.weight);
            }
        }

        public int Count => vertices.Count;

        public void AddVertex(int val)
        {
            int n = Count;

            //为vertices添加
            vertices.Add(val);

            //为adjMat添加
            //创建一行初始值为0的行
            List<float> newRow = new List<float>(n);
            for (int j = 0; j < n; j++)
            {
                newRow.Add(0);
            }
            adjMat.Add(newRow);//添加行
                               //添加列
            foreach (List<float> row in adjMat)
            {
                row.Add(0);//即对每一行尾添加0
            }
        }

        public void RemoveVertex(int index)
        {
            if (index >= Count) throw new Exception();

            //为vertices删除
            vertices.RemoveAt(index);
            //为adjMat删除
            adjMat.RemoveAt(index);//删除行
            foreach (List<float> row in adjMat)//删除列
            {
                row.RemoveAt(index);
            }
        }

        public void AddEdge(Edge edge)
        {
            int i = edge.vertex1, j = edge.vertex2;
            float weight = edge.weight;
            AddEdge(i, j, weight);
        }
        public void AddEdge(int i, int j, float weight = 1.0f)
        {
            //索引越界|中轴线，都不符合要求
            if (i < 0 || j < 0 || i >= Count || j >= Count || i == j)
                throw new Exception();

            adjMat[i][j] = weight;
            adjMat[j][i] = weight;
        }

        public void RemoveEdge(int i, int j)
        {
            //索引越界|中轴线，都不符合要求
            if (i < 0 || j < 0 || i >= Count || j >= Count || i == j)
                throw new Exception();

            adjMat[i][j] = 0;
            adjMat[j][i] = 0;
        }
    }

    public static class MUndirGraphAdjMatExtension
    {
        public static void Print(this MUndirGraphAdjMat graph)
        {
            MLog.Print("输出: ");

            if (graph.Count == 0)
            {
                MLog.Print("无元素");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"   {string.Join("     \t", graph.vertices)}\n");
            for (int i = 0; i < graph.vertices.Count; i++)
            {
                sb.Append($"      {graph.vertices[i]}：");
                for (int j = 0; j < graph.vertices.Count; j++)
                {
                    sb.Append(graph.adjMat[i][j] + "     \t");
                }
                sb.Append("\n");
            }
            MLog.Print(sb.ToString());
        }
    }
}