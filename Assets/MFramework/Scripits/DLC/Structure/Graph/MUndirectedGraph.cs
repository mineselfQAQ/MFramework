using System;
using System.Collections.Generic;
using System.Linq;

namespace MFramework.DLC
{
    /// <summary>
    /// 无向邻接列表
    /// </summary>
    public class MUndirectedGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// 字典---顶点所对应的边列表
        /// </summary>
        private IVertexEdgeDictionary<TVertex, TEdge> _edgeDic = new VertexEdgeDictionary<TVertex, TEdge>();
        private IList<TEdge> _edges = new List<TEdge>();

        public bool IsDirected => false;

        public bool IsVerticesEmpty => _edgeDic.Count == 0;
        public int VertexCount => _edgeDic.Count;
        public bool IsEdgesEmpty => EdgeCount == 0;
        public int EdgeCount { get; private set; }

        public MUndirectedGraph() { }

        public IEnumerable<TVertex> Vertices => _edgeDic.Keys.AsEnumerable();
        public IEnumerable<TEdge> Edges => _edges.AsEnumerable();

        #region 结构基础方法
        public bool AddVertex(TVertex vertex)
        {
            if (!ContainsVertex(vertex))
            {
                //为顶点添加边列表(即顶点->各个边)
                _edgeDic.Add(vertex, new EdgeList<TVertex, TEdge>());
                return true;
            }
            return false;
        }
        public bool RemoveVertex(TVertex vertex)
        {
            RemoveEdges(vertex);

            return _edgeDic.Remove(vertex);
        }

        /// <summary>
        /// 添加边  注意：由于只用一个TEdge添加，两顶点的显示是一致的
        /// </summary>
        public bool AddEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            IEdgeList<TVertex, TEdge> sourceEdges;
            IEdgeList<TVertex, TEdge> targetEdges;
            if (!_edgeDic.TryGetValue(edge.Source, out sourceEdges))
            {
                AddVertex(edge.Source);
            }
            if (!_edgeDic.TryGetValue(edge.Target, out targetEdges))
            {
                AddVertex(edge.Target);
            }

            //自环不应该添加
            if (edge.IsSelfEdge())
            {
                return false;
            }

            _edges.Add(edge);
            sourceEdges.Add(edge);
            targetEdges.Add(edge);

            ++EdgeCount;

            return true;
        }
        public void RemoveEdges(TVertex vertex)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            //通过顶点找到对应边列表
            if (_edgeDic.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> adjacentEdges))
            {
                IEdgeList<TVertex, TEdge> edgesToRemove = adjacentEdges.Clone();//暂存
                adjacentEdges.Clear();//先删除属于顶点的列表
                EdgeCount -= edgesToRemove.Count;
                if (EdgeCount < 0) throw new Exception();

                //再在顶点的另一边的顶点上删除该边
                foreach (TEdge edge in edgesToRemove)
                {
                    if (_edgeDic.TryGetValue(edge.Target, out adjacentEdges))
                    {
                        adjacentEdges.Remove(edge);
                    }
                    if (_edgeDic.TryGetValue(edge.Source, out adjacentEdges))
                    {
                        adjacentEdges.Remove(edge);
                    }

                    _edges.Remove(edge);
                }
            }
        }

        /// <summary>
        /// 获取顶点所连的所有边
        /// </summary>
        public IEnumerable<TEdge> GetAdjacentEdges(TVertex vertex)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            if (_edgeDic.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> edges))
                return edges.AsEnumerable();
            throw new Exception();
        }
        /// <summary>
        /// 获取顶点的度(连了几条边)
        /// </summary>
        public int GetAdjacentDegree(TVertex vertex)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            if (_edgeDic.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> edges))
                return edges.Count();
            throw new Exception();
        }
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));

            edge = default;
            if (_edgeDic.TryGetValue(source, out IEdgeList<TVertex, TEdge> adjacentEdges))
            {
                foreach (TEdge adjacentEdge in adjacentEdges)
                {
                    if ((EqualityComparer<TVertex>.Default.Equals(adjacentEdge.Source, source) &&
                         EqualityComparer<TVertex>.Default.Equals(adjacentEdge.Target, target)) ||
                        (EqualityComparer<TVertex>.Default.Equals(adjacentEdge.Source, target) &&
                         EqualityComparer<TVertex>.Default.Equals(adjacentEdge.Target, source)))
                    {
                        edge = adjacentEdge;//找到目标边，返回该边
                        return true;
                    }
                }
            }

            return false;
        }
        public TEdge GetEdge(TVertex source, TVertex target)
        {
            if (TryGetEdge(source, target, out TEdge edge))
            {
                return edge;
            }
            return default;
        }

        public bool ContainsVertex(TVertex vertex)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            return _edgeDic.ContainsKey(vertex);
        }
        public bool ContainsEdge(TEdge edge)
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));

            //以Source作为顶点(肯定Source和Target都可以)，查看边列表
            //遍历每一条边，查看是否有edge
            bool flag1 = _edgeDic.TryGetValue(edge.Source, out IEdgeList<TVertex, TEdge> adjacentEdges);
            bool flag2 = adjacentEdges.Any(adjacentEdge => EqualityComparer<TEdge>.Default.Equals(adjacentEdge, edge));

            return flag1 && flag2;
        }
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            return TryGetEdge(source, target, out _);
        }
        #endregion

        #region 算法
        public List<TVertex> BFS()
        {
            List<TVertex> res = new List<TVertex>();

            HashSet<TVertex> visited = new HashSet<TVertex>();
            foreach (var vertex in Vertices)
            {
                if (visited.Contains(vertex)) continue;

                visited.Add(vertex);
                var vertexRes = BFSInternal(vertex, visited);
                res.AddRange(vertexRes);
            }

            return res;
        }
        public List<TVertex> BFS(TVertex startVertex)
        {
            HashSet<TVertex> visited = new HashSet<TVertex>() { startVertex };
            return BFSInternal(startVertex, visited);
        }
        private List<TVertex> BFSInternal(TVertex startVertex, HashSet<TVertex> visited)
        {
            List<TVertex> res = new List<TVertex>();

            Queue<TVertex> queue = new Queue<TVertex>();
            queue.Enqueue(startVertex);
            while (queue.Count > 0)
            {
                TVertex vertex = queue.Dequeue();
                res.Add(vertex);

                foreach (var edge in _edgeDic[vertex])
                {
                    TVertex other = default;
                    if (EqualityComparer<TVertex>.Default.Equals(edge.Source, vertex))
                        other = edge.Target;
                    else if (EqualityComparer<TVertex>.Default.Equals(edge.Target, vertex))
                        other = edge.Source;

                    if (visited.Contains(other)) continue;

                    queue.Enqueue(other);
                    visited.Add(other);
                }
            }
            return res;
        }

        public List<TVertex> DFS()
        {
            List<TVertex> res = new List<TVertex>();
            HashSet<TVertex> visited = new HashSet<TVertex>();

            foreach (var vertex in Vertices)
            {
                if (visited.Contains(vertex)) continue;

                DFSInternal(vertex, res, visited);
            }
            return res;
        }
        public List<TVertex> DFS(TVertex vertex)
        {
            List<TVertex> res = new List<TVertex>();
            HashSet<TVertex> visited = new HashSet<TVertex>();

            DFSInternal(vertex, res, visited);
            return res;
        }
        private void DFSInternal(TVertex vertex, List<TVertex> res, HashSet<TVertex> visited)
        {
            if (visited.Contains(vertex)) return;

            visited.Add(vertex);
            res.Add(vertex);

            foreach (var edge in _edgeDic[vertex])
            {
                TVertex other = default;
                if (EqualityComparer<TVertex>.Default.Equals(edge.Source, vertex))
                    other = edge.Target;
                else if (EqualityComparer<TVertex>.Default.Equals(edge.Target, vertex))
                    other = edge.Source;

                DFSInternal(other, res, visited);
            }
        }

        public bool HasCycle()
        {
            HashSet<TVertex> visited = new HashSet<TVertex>();
            HashSet<TVertex> pathSet = new HashSet<TVertex>();

            foreach (var vertex in Vertices)
            {
                if (HasCycleDFS(vertex, default, visited, pathSet))
                    return true;
            }

            return false;
        }
        private bool HasCycleDFS(TVertex vertex, TVertex parent, HashSet<TVertex> visited, HashSet<TVertex> pathSet)
        {
            if (pathSet.Contains(vertex)) return true;//已存在说明出现了环

            if (visited.Contains(vertex)) return false;//访问过就返回

            visited.Add(vertex);
            pathSet.Add(vertex);

            foreach (var edge in _edgeDic[vertex])
            {
                TVertex other = default;
                if (EqualityComparer<TVertex>.Default.Equals(edge.Source, vertex))
                    other = edge.Target;
                else if (EqualityComparer<TVertex>.Default.Equals(edge.Target, vertex))
                    other = edge.Source;

                //如果连接顶点就是上一节点，则不进行
                //如A-B-C，从A到了B，现在B如果回A的话是一个环路且没有意义
                if (EqualityComparer<TVertex>.Default.Equals(other, parent)) continue;
                if (HasCycleDFS(edge.Target, vertex, visited, pathSet)) return true;
            }

            pathSet.Remove(vertex);//顶点寻找结束就路径回退
            return false;
        }

        public List<List<TVertex>> FindCycle()
        {
            //已访问节点(无用)
            //如：先进行0->1->2->0检测，此时0/1/2都会加入visited，第二个环为0->1->3->4->2->0，遇到2就退出了
            //所以仅用于判断是否顶点为孤岛(也许有其它情况？但是没有访问过说明出现了新的可能)
            HashSet<TVertex> visited = new HashSet<TVertex>();
            //路径记录
            List<TVertex> pathList = new List<TVertex>();//Tip：如果只是判断是否存在，只需要使用HashSet即可
            //输出环列表
            List<List<TVertex>> allCycles = new List<List<TVertex>>();
            //归一化字符串
            HashSet<string> uniqueCycles = new HashSet<string>();

            //遍历所有顶点寻找环
            foreach (var vertex in Vertices)
            {
                if (!visited.Contains(vertex))//如果没有访问过才说明是新的岛屿
                    FindCycleDFS(vertex, default, visited, pathList, allCycles, uniqueCycles);
            }

            return allCycles;
        }
        private void FindCycleDFS(TVertex vertex, TVertex parent, HashSet<TVertex> visitedVertex, List<TVertex> pathList, List<List<TVertex>> allCycles, HashSet<string> uniqueCycles)
        {
            //路径中出现相同，说明出现环
            //如：A->B->C->A，出现A时说明环出现
            if (pathList.Contains(vertex))
            {
                //由于路径可能为A->B->C->D->B，所以需要找到环的头
                var cycleStartIndex = pathList.IndexOf(vertex);
                var cycle = new List<TVertex>(pathList.GetRange(cycleStartIndex, pathList.Count - cycleStartIndex));

                string[] normalizeCycle = NormalizeCycle(cycle);
                if (!uniqueCycles.Contains(normalizeCycle[0]))
                {
                    //字符串存2个(一正一反)
                    uniqueCycles.Add(normalizeCycle[0]);
                    uniqueCycles.Add(normalizeCycle[1]);
                    cycle.Add(vertex);//添加头尾元素(如A->B->C->A)
                    allCycles.Add(cycle);
                }

                return;
            }

            //访问则添加
            pathList.Add(vertex);
            if (!visitedVertex.Contains(vertex)) visitedVertex.Add(vertex);

            //遍历对所有连接顶点递归进入
            foreach (var edge in _edgeDic[vertex])
            {
                TVertex next = default;
                if (EqualityComparer<TVertex>.Default.Equals(edge.Source, vertex))
                    next = edge.Target;
                else if (EqualityComparer<TVertex>.Default.Equals(edge.Target, vertex))
                    next = edge.Source;

                if (EqualityComparer<TVertex>.Default.Equals(next, parent)) continue;
                FindCycleDFS(next, vertex, visitedVertex, pathList, allCycles, uniqueCycles);
            }

            pathList.RemoveAt(pathList.Count - 1);//此轮结束，路径回退
        }
        private string[] NormalizeCycle(List<TVertex> cycle)
        {
            //寻找环中最小的起点
            int minIndex = 0;
            for (int i = 1; i < cycle.Count; i++)
            {
                if (Comparer<TVertex>.Default.Compare(cycle[i], cycle[minIndex]) < 0)
                    minIndex = i;
            }

            //重新排列环，以最小值为起点
            List<TVertex> normalized = new List<TVertex>();
            for (int i = 0; i < cycle.Count; i++)
            {
                normalized.Add(cycle[(minIndex + i) % cycle.Count]);
            }

            //搭建反向字符串
            List<TVertex> reverseNormalized = new List<TVertex>();
            reverseNormalized.Add(normalized[0]);
            for (int i = normalized.Count - 1; i >= 1; i--)
            {
                reverseNormalized.Add(normalized[i]);
            }

            //返回规范化的环表示
            string res1 = string.Join("->", normalized);
            string res2 = string.Join("->", reverseNormalized);
            return new string[] { res1, res2 };
        }
        #endregion
    }
}