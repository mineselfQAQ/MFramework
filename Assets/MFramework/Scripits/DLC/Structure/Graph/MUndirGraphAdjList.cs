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

        /// <summary>
        /// 寻找是否有环
        /// </summary>
        public bool HasCycle()
        {
            HashSet<int> visited = new HashSet<int>();
            HashSet<int> pathSet = new HashSet<int>();

            foreach (var vertex in adjList.Keys)
            {
                if (HasCycleDFS(vertex, -1, visited, pathSet))
                    return true;
            }

            return false;
        }
        private bool HasCycleDFS(int vertex, int parent, HashSet<int> visited, HashSet<int> pathSet)
        {
            if (pathSet.Contains(vertex)) return true;

            if (visited.Contains(vertex)) return false;

            visited.Add(vertex);
            pathSet.Add(vertex);

            foreach (var neighbor in adjList[vertex])
            {
                if (neighbor.index == parent) continue;
                if (HasCycleDFS(neighbor.index, vertex, visited, pathSet)) return true;
            }

            pathSet.Remove(vertex);
            return false;
        }

        /// <summary>
        /// 寻找无向图中的环
        /// </summary>
        public List<List<int>> FindCycle()
        {
            List<List<int>> allCycles = new List<List<int>>();
            HashSet<string> uniqueCycles = new HashSet<string>();

            foreach (var vertex in adjList.Keys)
            {
                List<int> pathList = new List<int>();
                FindCycleDFS(vertex, -1, pathList, allCycles, uniqueCycles);
            }

            return allCycles;
        }
        private void FindCycleDFS(int vertex, int parent, List<int> pathList,
            List<List<int>> allCycles, HashSet<string> uniqueCycles)
        {
            pathList.Add(vertex);

            foreach (var neighbor in adjList[vertex])
            {
                //忽略回到父节点的边(无向图中避免来回走动)
                if (neighbor.index == parent) continue;

                if (pathList.Contains(neighbor.index))
                {
                    int cycleStartIndex = pathList.IndexOf(neighbor.index);
                    var cycle = new List<int>(pathList.GetRange(cycleStartIndex, pathList.Count - cycleStartIndex));

                    string[] normalizeCycle = NormalizeCycle(cycle);
                    if (!uniqueCycles.Contains(normalizeCycle[0]))
                    {
                        uniqueCycles.Add(normalizeCycle[0]);
                        uniqueCycles.Add(normalizeCycle[1]);
                        cycle.Add(neighbor.index);
                        allCycles.Add(cycle);
                    }
                }
                else
                {
                    FindCycleDFS(neighbor.index, vertex, pathList, allCycles, uniqueCycles);
                }
            }

            pathList.RemoveAt(pathList.Count - 1);
        }
        private string[] NormalizeCycle(List<int> cycle)
        {
            //寻找环中最小的起点
            int minIndex = 0;
            for (int i = 1; i < cycle.Count; i++)
            {
                if (cycle[i] < cycle[minIndex]) minIndex = i;
            }

            //重新排列环，以最小值为起点
            List<int> normalized = new List<int>();
            for (int i = 0; i < cycle.Count; i++)
            {
                normalized.Add(cycle[(minIndex + i) % cycle.Count]);
            }

            List<int> reverseNormalized = new List<int>();
            reverseNormalized.Add(normalized[0]);
            for (int i = normalized.Count - 1; i >= 1; i--)
            {
                reverseNormalized.Add(normalized[i]);
            }

            // 返回规范化的环表示
            string res1 = string.Join("->", normalized);
            string res2 = string.Join("->", reverseNormalized);
            return new string[] { res1, res2 };
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