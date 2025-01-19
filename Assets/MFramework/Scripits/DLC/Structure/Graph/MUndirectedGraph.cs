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
        #endregion
    }
}