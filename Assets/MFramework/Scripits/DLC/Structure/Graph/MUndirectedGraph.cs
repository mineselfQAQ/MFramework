using System;
using System.Collections.Generic;
using System.Linq;

namespace MFramework.DLC
{
    /// <summary>
    /// �����ڽ��б�
    /// </summary>
    public class MUndirectedGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// �ֵ�---��������Ӧ�ı��б�
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

        #region �ṹ��������
        public bool AddVertex(TVertex vertex)
        {
            if (!ContainsVertex(vertex))
            {
                //Ϊ������ӱ��б�(������->������)
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
        /// ��ӱ�  ע�⣺����ֻ��һ��TEdge��ӣ����������ʾ��һ�µ�
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

            //�Ի���Ӧ�����
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

            //ͨ�������ҵ���Ӧ���б�
            if (_edgeDic.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> adjacentEdges))
            {
                IEdgeList<TVertex, TEdge> edgesToRemove = adjacentEdges.Clone();//�ݴ�
                adjacentEdges.Clear();//��ɾ�����ڶ�����б�
                EdgeCount -= edgesToRemove.Count;
                if (EdgeCount < 0) throw new Exception();

                //���ڶ������һ�ߵĶ�����ɾ���ñ�
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
        /// ��ȡ�������������б�
        /// </summary>
        public IEnumerable<TEdge> GetAdjacentEdges(TVertex vertex)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            if (_edgeDic.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> edges))
                return edges.AsEnumerable();
            throw new Exception();
        }
        /// <summary>
        /// ��ȡ����Ķ�(���˼�����)
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
                        edge = adjacentEdge;//�ҵ�Ŀ��ߣ����ظñ�
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

            //��Source��Ϊ����(�϶�Source��Target������)���鿴���б�
            //����ÿһ���ߣ��鿴�Ƿ���edge
            bool flag1 = _edgeDic.TryGetValue(edge.Source, out IEdgeList<TVertex, TEdge> adjacentEdges);
            bool flag2 = adjacentEdges.Any(adjacentEdge => EqualityComparer<TEdge>.Default.Equals(adjacentEdge, edge));

            return flag1 && flag2;
        }
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            return TryGetEdge(source, target, out _);
        }
        #endregion

        #region �㷨
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
            if (pathSet.Contains(vertex)) return true;//�Ѵ���˵�������˻�

            if (visited.Contains(vertex)) return false;//���ʹ��ͷ���

            visited.Add(vertex);
            pathSet.Add(vertex);

            foreach (var edge in _edgeDic[vertex])
            {
                TVertex other = default;
                if (EqualityComparer<TVertex>.Default.Equals(edge.Source, vertex))
                    other = edge.Target;
                else if (EqualityComparer<TVertex>.Default.Equals(edge.Target, vertex))
                    other = edge.Source;

                //������Ӷ��������һ�ڵ㣬�򲻽���
                //��A-B-C����A����B������B�����A�Ļ���һ����·��û������
                if (EqualityComparer<TVertex>.Default.Equals(other, parent)) continue;
                if (HasCycleDFS(edge.Target, vertex, visited, pathSet)) return true;
            }

            pathSet.Remove(vertex);//����Ѱ�ҽ�����·������
            return false;
        }
        #endregion
    }
}