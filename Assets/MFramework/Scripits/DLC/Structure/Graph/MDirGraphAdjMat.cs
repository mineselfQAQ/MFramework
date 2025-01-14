using System;
using System.Collections.Generic;
using System.Text;

namespace MFramework.DLC
{
    /// <summary>
    /// 有向有权邻接矩阵图(定义：行到列)
    /// </summary>
    public class MDirGraphAdjMat
    {
        public struct Edge
        {
            public int startVertex;
            public int endVertex;
            public float weight;

            public Edge(int startVertex, int endVertex, float weight = 1.0f)
            {
                this.startVertex = startVertex;
                this.endVertex = endVertex;
                this.weight = weight;
            }
        }

        internal List<int> vertices;//顶点与对应id
        internal List<List<float>> adjMat;//邻接矩阵

        public MDirGraphAdjMat()
        {
            this.vertices = new List<int>();
            this.adjMat = new List<List<float>>();
        }
        /// <param name="edges">n行2/3列矩阵 2列为无权 3列为有权</param>
        public MDirGraphAdjMat(int[] vertices, Edge[] edges)
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
                AddEdge(e);
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
            int from = edge.startVertex, to = edge.endVertex;
            float weight = edge.weight;
            AddEdge(from, to, weight);
        }
        public void AddEdge(int from, int to, float weight = 1.0f)
        {
            //索引越界，不符合要求
            //Tip：i=j不止可能为0，如果为1说明是自环
            if (from < 0 || to < 0 || from >= Count || to >= Count)
                throw new Exception();

            adjMat[from][to] = weight;
        }

        public void RemoveEdge(int from, int to)
        {
            //索引越界，不符合要求
            //Tip：i=j不止可能为0，如果为1说明是自环
            if (from < 0 || to < 0 || from >= Count || to >= Count)
                throw new Exception();

            adjMat[from][to] = 0;
        }
    }

    public static class MDirGraphAdjMatExtension
    {
        public static void Print(this MDirGraphAdjMat graph)
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