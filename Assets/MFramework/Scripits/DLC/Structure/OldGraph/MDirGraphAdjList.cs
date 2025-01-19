using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting.Antlr3.Runtime.Collections;

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

        /// <summary>
        /// 寻找是否有环
        /// </summary>
        public bool HasCycle()
        {
            HashSet<int> visited = new HashSet<int>();
            HashSet<int> pathSet = new HashSet<int>();

            foreach (var vertex in adjList.Keys)
            {
                if (HasCycleDFS(vertex, visited, pathSet))
                    return true;
            }

            return false;
        }
        private bool HasCycleDFS(int vertex, HashSet<int> visited, HashSet<int> pathSet)
        {
            if (pathSet.Contains(vertex)) return true;

            if (visited.Contains(vertex)) return false;

            visited.Add(vertex);
            pathSet.Add(vertex);

            foreach (var neighbor in adjList[vertex])
            {
                if (HasCycleDFS(neighbor.index, visited, pathSet)) return true;
            }

            pathSet.Remove(vertex);
            return false;
        }

        /// <summary>
        /// 寻找表中的环
        /// </summary>
        public List<List<int>> FindCycle()
        {
            //已访问节点(无用)
            //如：先进行0->1->2->0检测，此时0/1/2都会加入visited，第二个环为0->1->3->4->2->0，遇到2就退出了
            //HashSet<int> visited = new HashSet<int>();
            //路径记录
            List<int> pathList = new List<int>();//Tip：如果只是判断是否存在，只需要使用HashSet即可
            //输出环列表
            List<List<int>> allCycles = new List<List<int>>();
            //环的字符串表示形式
            HashSet<string> uniqueCycles = new HashSet<string>();

            //遍历所有顶点寻找环
            foreach (var vertex in adjList.Keys)
            {
                FindCycleDFS(vertex, pathList, allCycles, uniqueCycles);
            }

            return allCycles;
        }
        private void FindCycleDFS(int vertex, List<int> pathList, List<List<int>> allCycles, HashSet<string> uniqueCycles)
        {
            //路径中出现相同，说明出现环
            //如：A->B->C->A，出现A时说明环出现
            if (pathList.Contains(vertex))
            {
                //由于路径可能为A->B->C->D->B，所以需要找到环的头
                var cycleStartIndex = pathList.IndexOf(vertex);
                var cycle = new List<int>(pathList.GetRange(cycleStartIndex, pathList.Count - cycleStartIndex));

                //检测环是否为新环(如：0->1->2->0和1->2->0->1是同一环)
                //需要在添加为环之前检测，否则重复元素不同
                string normalizeCycle = NormalizeCycle(cycle);
                if (uniqueCycles.Add(normalizeCycle))
                {
                    cycle.Add(vertex);
                    allCycles.Add(cycle);
                }

                return;
            }

            //访问则添加
            pathList.Add(vertex);

            //遍历对所有连接顶点递归进入
            foreach (var neighbor in adjList[vertex])
            {
                FindCycleDFS(neighbor.index, pathList, allCycles, uniqueCycles);
            }

            pathList.RemoveAt(pathList.Count - 1);//此轮结束，路径回退
        }
        private string NormalizeCycle(List<int> cycle)
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

            //转换为字符串作为唯一标识
            return string.Join("->", normalized);
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