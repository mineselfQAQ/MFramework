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
        private IVertexEdgeDictionary<TVertex, TEdge> _adjacentEdges = new VertexEdgeDictionary<TVertex, TEdge>();
        private IList<TEdge> _edges = new List<TEdge>();

        public bool IsDirected => false;

        public bool IsVerticesEmpty => _adjacentEdges.Count == 0;
        public int VertexCount => _adjacentEdges.Count;
        public bool IsEdgesEmpty => EdgeCount == 0;
        public int EdgeCount { get; private set; }

        public MUndirectedGraph() { }

        public IEnumerable<TVertex> Vertices => _adjacentEdges.Keys.AsEnumerable();
        public IEnumerable<TEdge> Edges => _edges.AsEnumerable();

        public bool AddVertex(TVertex vertex)
        {
            if (!ContainsVertex(vertex))
            {
                //Ϊ������ӱ��б�(������->������)
                _adjacentEdges.Add(vertex, new EdgeList<TVertex, TEdge>());
                return true;
            }
            return false;
        }
        public bool RemoveVertex(TVertex vertex)
        {
            RemoveEdges(vertex);

            return _adjacentEdges.Remove(vertex);
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
            if (!_adjacentEdges.TryGetValue(edge.Source, out sourceEdges))
            {
                AddVertex(edge.Source);
            }
            if (!_adjacentEdges.TryGetValue(edge.Target, out targetEdges))
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
            if (_adjacentEdges.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> adjacentEdges))
            {
                IEdgeList<TVertex, TEdge> edgesToRemove = adjacentEdges.Clone();//�ݴ�
                adjacentEdges.Clear();//��ɾ�����ڶ�����б�
                EdgeCount -= edgesToRemove.Count;
                if (EdgeCount < 0) throw new Exception();

                //���ڶ������һ�ߵĶ�����ɾ���ñ�
                foreach (TEdge edge in edgesToRemove)
                {
                    if (_adjacentEdges.TryGetValue(edge.Target, out adjacentEdges))
                    {
                        adjacentEdges.Remove(edge);
                    }
                    if (_adjacentEdges.TryGetValue(edge.Source, out adjacentEdges))
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

            if (_adjacentEdges.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> edges))
                return edges.AsEnumerable();
            throw new Exception();
        }
        /// <summary>
        /// ��ȡ����Ķ�(���˼�����)
        /// </summary>
        public int GetAdjacentDegree(TVertex vertex)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            if (_adjacentEdges.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> edges))
                return edges.Count();
            throw new Exception();
        }
        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));

            edge = default;
            if (_adjacentEdges.TryGetValue(source, out IEdgeList<TVertex, TEdge> adjacentEdges))
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

            return _adjacentEdges.ContainsKey(vertex);
        }
        public bool ContainsEdge(TEdge edge)
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));

            //��Source��Ϊ����(�϶�Source��Target������)���鿴���б�
            //����ÿһ���ߣ��鿴�Ƿ���edge
            bool flag1 = _adjacentEdges.TryGetValue(edge.Source, out IEdgeList<TVertex, TEdge> adjacentEdges);
            bool flag2 = adjacentEdges.Any(adjacentEdge => EqualityComparer<TEdge>.Default.Equals(adjacentEdge, edge));

            return flag1 && flag2;
        }
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            return TryGetEdge(source, target, out _);
        }
    }
}