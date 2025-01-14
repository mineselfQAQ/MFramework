using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace MFramework.DLC
{
    /// <summary>
    /// 无向有权邻接表图
    /// </summary>
    public class MUndirGraphAdjList
    {
        public struct Vertex
        {
            public int index;
            public float weight;

            public Vertex(int index, float weight = 1.0f)
            {
                this.index = index;
                this.weight = weight;
            }

            public override string ToString()
            {
                return $"{index}({weight})";
            }

            public override bool Equals(object obj)
            {
                if (obj is Vertex other)
                {
                    return this.index == other.index;
                }
                else if (obj is int val)
                {
                    return this.index == val;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return index.GetHashCode();
            }
        }
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

        public Dictionary<int, HashSet<Vertex>> adjList;

        public MUndirGraphAdjList()
        {
            adjList = new Dictionary<int, HashSet<Vertex>>();
        }
        public MUndirGraphAdjList(Edge[] edges)
        {
            adjList = new Dictionary<int, HashSet<Vertex>>();
            //添加所有顶点和边
            foreach (var edge in edges)
            {
                AddVertex(edge.vertex1);
                AddVertex(edge.vertex2);
                AddEdge(edge.vertex1, edge.vertex2, edge.weight);
            }
        }

        public int Count => adjList.Count;

        public void AddEdge(int vet1, int vet2, float weight = 1.0f)
        {
            if (!adjList.ContainsKey(vet1) || !adjList.ContainsKey(vet2) || vet1 == vet2)
                throw new Exception();

            adjList[vet1].Add(new Vertex(vet2, weight));
            adjList[vet2].Add(new Vertex(vet1, weight));
        }

        public void RemoveEdge(int vet1, int vet2)
        {
            if (!adjList.ContainsKey(vet1) || !adjList.ContainsKey(vet2) || vet1 == vet2)
                throw new Exception();

            Vertex temp = new Vertex(vet2);
            if (adjList[vet1].Contains(temp))
            {
                adjList[vet1].Remove(temp);
            }
            temp = new Vertex(vet1);
            if (adjList[vet2].Contains(temp))
            {
                adjList[vet2].Remove(temp);
            }
        }

        public void AddVertex(int vet)
        {
            if (adjList.ContainsKey(vet)) return;

            adjList.Add(vet, new HashSet<Vertex>());
        }

        public void RemoveVertex(int vet)
        {
            if (!adjList.ContainsKey(vet)) throw new Exception();

            //删除顶点List
            adjList.Remove(vet);
            //删除每个List中的顶点
            foreach (HashSet<Vertex> list in adjList.Values)
            {
                Vertex temp = new Vertex(vet);
                if (list.Contains(temp))
                {
                    list.Remove(temp);
                }
            }
        }

        public List<int> BFS()
        {
            //输出结果
            List<int> res = new List<int>();
            //已访问的顶点
            HashSet<int> visited = new HashSet<int>();

            foreach (var index in adjList.Keys)
            {
                if (visited.Contains(index)) continue;

                Queue<int> queue = new Queue<int>();
                queue.Enqueue(index);
                visited.Add(index);

                while (queue.Count > 0)
                {
                    int val = queue.Dequeue();
                    res.Add(val);

                    if (adjList.ContainsKey(val))
                    {
                        foreach (var v in adjList[val])
                        {
                            if (visited.Contains(v.index)) continue;

                            queue.Enqueue(v.index);
                            visited.Add(v.index);
                        }
                    }
                }
            }
            return res;
        }
        public List<int> BFS(int startVet)
        {
            //输出List
            List<int> res = new List<int>();
            //已访问顶点
            HashSet<int> visited = new HashSet<int>() { startVet };

            Queue<int> queue = new Queue<int>();
            queue.Enqueue(startVet);
            while (queue.Count > 0)
            {
                int val = queue.Dequeue();
                res.Add(val);

                foreach (var v in adjList[val])
                {
                    if (visited.Contains(v.index)) continue;

                    queue.Enqueue(v.index);
                    visited.Add(v.index);
                }
            }

            return res;
        }

        public List<int> DFS()
        {
            List<int> res = new List<int>();
            HashSet<int> visited = new HashSet<int>();

            foreach (var index in adjList.Keys)
            {
                if (visited.Contains(index)) continue;
                DFS(visited, res, index);
            }
            return res;
        }
        public List<int> DFS(HashSet<int> visited, List<int> res, int startVet)
        {
            DFSInternal(visited, res, startVet);
            return res;
        }
        private void DFSInternal(HashSet<int> visited, List<int> res, int index)
        {
            res.Add(index);
            visited.Add(index);
            foreach (var v in adjList[index])
            {
                if (visited.Contains(v.index)) continue;
                DFSInternal(visited, res, v.index);
            }
        }
    }

    public static class MUndirGraphAdjListExtension
    {
        public static void Print(this MUndirGraphAdjList graph)
        {
            MLog.Print("输出: ");

            if (graph.Count == 0)
            {
                MLog.Print("无元素");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("\n");
            foreach (var pair in graph.adjList)
            {
                sb.Append($"{pair.Key.ToString().PadRight(2)}：");
                foreach (var vet in pair.Value)
                {
                    sb.Append(vet.ToString().PadRight(8));
                }
                sb.Append('\n');
            }
            MLog.Print(sb.ToString());
        }
    }
}