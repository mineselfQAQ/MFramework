using System;
using System.Collections.Generic;
using System.Text;

namespace MFramework.DLC
{
    /// 有向有权邻接表图
    public class MDirGraphAdjList
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

        public Dictionary<int, HashSet<Vertex>> adjList;

        public MDirGraphAdjList()
        {
            adjList = new Dictionary<int, HashSet<Vertex>>();
        }
        public MDirGraphAdjList(Edge[] edges)
        {
            adjList = new Dictionary<int, HashSet<Vertex>>();
            //添加所有顶点和边
            foreach (var edge in edges)
            {
                AddVertex(edge.startVertex);
                AddVertex(edge.endVertex);
                AddEdge(edge.startVertex, edge.endVertex, edge.weight);
            }
        }

        public int Count => adjList.Count;

        public void AddEdge(Edge edge)
        {
            int from = edge.startVertex, to = edge.endVertex;
            float weight = edge.weight;
            AddEdge(from, to, weight);
        }
        public void AddEdge(int from, int to, float weight = 1.0f)
        {
            if (!adjList.ContainsKey(from) || !adjList.ContainsKey(to))
                throw new Exception();

            adjList[from].Add(new Vertex(to, weight));
        }

        public void RemoveEdge(int from, int to)
        {
            if (!adjList.ContainsKey(from) || !adjList.ContainsKey(to))
                throw new Exception();

            Vertex temp = new Vertex(to);
            if (adjList[from].Contains(temp))
            {
                adjList[from].Remove(temp);
            }
        }

        public void AddVertex(int vet)
        {
            if (adjList.ContainsKey(vet)) return;

            adjList.Add(vet, new HashSet<Vertex>());
        }

        public void RemoveVertex(int vet)
        {
            if (!adjList.ContainsKey(vet))
                throw new Exception();

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

        public bool HasCycle()
        {
            HashSet<int> visited = new HashSet<int>();
            HashSet<int> recStack = new HashSet<int>();

            foreach (var vertex in adjList.Keys)
            {
                if (HasCycleDFS(vertex, visited, recStack))
                    return true;
            }

            return false;
        }
        private bool HasCycleDFS(int vertex, HashSet<int> visited, HashSet<int> recStack)
        {
            if (recStack.Contains(vertex))
                return true; // 发现环

            if (visited.Contains(vertex))
                return false; // 已访问，不是环

            visited.Add(vertex);
            recStack.Add(vertex);

            foreach (var neighbor in adjList[vertex])
            {
                if (HasCycleDFS(neighbor.index, visited, recStack))
                    return true;
            }

            recStack.Remove(vertex); // 当前路径结束，移出递归栈
            return false;
        }
    }

    public static class MDirGraphAdjListExtension
    {
        public static void Print(this MDirGraphAdjList graph)
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