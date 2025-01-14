using System;
using System.Collections.Generic;
using System.Text;

namespace MFramework.DLC
{
    /// <summary>
    /// ������Ȩ�ڽӾ���ͼ
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

        internal List<int> vertices;//�������Ӧid
        internal List<List<float>> adjMat;//�ڽӾ���

        public MUndirGraphAdjMat()
        {
            this.vertices = new List<int>();
            this.adjMat = new List<List<float>>();
        }
        public MUndirGraphAdjMat(int[] vertices, Edge[] edges)
        {
            this.vertices = new List<int>();
            this.adjMat = new List<List<float>>();

            //��Ӷ���
            foreach (int val in vertices)
            {
                AddVertex(val);
            }

            //��ӱ�
            foreach (var e in edges)
            {
                AddEdge(e.vertex1, e.vertex2, e.weight);
            }
        }

        public int Count => vertices.Count;

        public void AddVertex(int val)
        {
            int n = Count;

            //Ϊvertices���
            vertices.Add(val);

            //ΪadjMat���
            //����һ�г�ʼֵΪ0����
            List<float> newRow = new List<float>(n);
            for (int j = 0; j < n; j++)
            {
                newRow.Add(0);
            }
            adjMat.Add(newRow);//�����
                               //�����
            foreach (List<float> row in adjMat)
            {
                row.Add(0);//����ÿһ��β���0
            }
        }

        public void RemoveVertex(int index)
        {
            if (index >= Count) throw new Exception();

            //Ϊverticesɾ��
            vertices.RemoveAt(index);
            //ΪadjMatɾ��
            adjMat.RemoveAt(index);//ɾ����
            foreach (List<float> row in adjMat)//ɾ����
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
            //����Խ��|�����ߣ���������Ҫ��
            if (i < 0 || j < 0 || i >= Count || j >= Count || i == j)
                throw new Exception();

            adjMat[i][j] = weight;
            adjMat[j][i] = weight;
        }

        public void RemoveEdge(int i, int j)
        {
            //����Խ��|�����ߣ���������Ҫ��
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
            MLog.Print("���: ");

            if (graph.Count == 0)
            {
                MLog.Print("��Ԫ��");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"   {string.Join("     \t", graph.vertices)}\n");
            for (int i = 0; i < graph.vertices.Count; i++)
            {
                sb.Append($"      {graph.vertices[i]}��");
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