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

        public List<List<TVertex>> FindCycle()
        {
            //�ѷ��ʽڵ�(����)
            //�磺�Ƚ���0->1->2->0��⣬��ʱ0/1/2�������visited���ڶ�����Ϊ0->1->3->4->2->0������2���˳���
            //���Խ������ж��Ƿ񶥵�Ϊ�µ�(Ҳ�����������������û�з��ʹ�˵���������µĿ���)
            HashSet<TVertex> visited = new HashSet<TVertex>();
            //·����¼
            List<TVertex> pathList = new List<TVertex>();//Tip�����ֻ���ж��Ƿ���ڣ�ֻ��Ҫʹ��HashSet����
            //������б�
            List<List<TVertex>> allCycles = new List<List<TVertex>>();
            //��һ���ַ���
            HashSet<string> uniqueCycles = new HashSet<string>();

            //�������ж���Ѱ�һ�
            foreach (var vertex in Vertices)
            {
                if (!visited.Contains(vertex))//���û�з��ʹ���˵�����µĵ���
                    FindCycleDFS(vertex, default, visited, pathList, allCycles, uniqueCycles);
            }

            return allCycles;
        }
        private void FindCycleDFS(TVertex vertex, TVertex parent, HashSet<TVertex> visitedVertex, List<TVertex> pathList, List<List<TVertex>> allCycles, HashSet<string> uniqueCycles)
        {
            //·���г�����ͬ��˵�����ֻ�
            //�磺A->B->C->A������Aʱ˵��������
            if (pathList.Contains(vertex))
            {
                //����·������ΪA->B->C->D->B��������Ҫ�ҵ�����ͷ
                var cycleStartIndex = pathList.IndexOf(vertex);
                var cycle = new List<TVertex>(pathList.GetRange(cycleStartIndex, pathList.Count - cycleStartIndex));

                string[] normalizeCycle = NormalizeCycle(cycle);
                if (!uniqueCycles.Contains(normalizeCycle[0]))
                {
                    //�ַ�����2��(һ��һ��)
                    uniqueCycles.Add(normalizeCycle[0]);
                    uniqueCycles.Add(normalizeCycle[1]);
                    cycle.Add(vertex);//���ͷβԪ��(��A->B->C->A)
                    allCycles.Add(cycle);
                }

                return;
            }

            //���������
            pathList.Add(vertex);
            if (!visitedVertex.Contains(vertex)) visitedVertex.Add(vertex);

            //�������������Ӷ���ݹ����
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

            pathList.RemoveAt(pathList.Count - 1);//���ֽ�����·������
        }
        private string[] NormalizeCycle(List<TVertex> cycle)
        {
            //Ѱ�һ�����С�����
            int minIndex = 0;
            for (int i = 1; i < cycle.Count; i++)
            {
                if (Comparer<TVertex>.Default.Compare(cycle[i], cycle[minIndex]) < 0)
                    minIndex = i;
            }

            //�������л�������СֵΪ���
            List<TVertex> normalized = new List<TVertex>();
            for (int i = 0; i < cycle.Count; i++)
            {
                normalized.Add(cycle[(minIndex + i) % cycle.Count]);
            }

            //������ַ���
            List<TVertex> reverseNormalized = new List<TVertex>();
            reverseNormalized.Add(normalized[0]);
            for (int i = normalized.Count - 1; i >= 1; i--)
            {
                reverseNormalized.Add(normalized[i]);
            }

            //���ع淶���Ļ���ʾ
            string res1 = string.Join("->", normalized);
            string res2 = string.Join("->", reverseNormalized);
            return new string[] { res1, res2 };
        }
        #endregion
    }
}