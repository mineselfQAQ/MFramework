using System;
using System.Collections.Generic;
using System.Text;

namespace MFramework.DLC
{
    /// <summary>
    /// ������Ȩ�ڽӾ���ͼ(���壺�е���)
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

        internal List<int> vertices;//�������Ӧid
        internal List<List<float>> adjMat;//�ڽӾ���

        public MDirGraphAdjMat()
        {
            this.vertices = new List<int>();
            this.adjMat = new List<List<float>>();
        }
        /// <param name="edges">n��2/3�о��� 2��Ϊ��Ȩ 3��Ϊ��Ȩ</param>
        public MDirGraphAdjMat(int[] vertices, Edge[] edges)
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
                AddEdge(e);
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
            int from = edge.startVertex, to = edge.endVertex;
            float weight = edge.weight;
            AddEdge(from, to, weight);
        }
        public void AddEdge(int from, int to, float weight = 1.0f)
        {
            //����Խ�磬������Ҫ��
            //Tip��i=j��ֹ����Ϊ0�����Ϊ1˵�����Ի�
            if (from < 0 || to < 0 || from >= Count || to >= Count)
                throw new Exception();

            adjMat[from][to] = weight;
        }

        public void RemoveEdge(int from, int to)
        {
            //����Խ�磬������Ҫ��
            //Tip��i=j��ֹ����Ϊ0�����Ϊ1˵�����Ի�
            if (from < 0 || to < 0 || from >= Count || to >= Count)
                throw new Exception();

            adjMat[from][to] = 0;
        }
    }

    public static class MDirGraphAdjMatExtension
    {
        public static void Print(this MDirGraphAdjMat graph)
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