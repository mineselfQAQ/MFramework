using System;
using System.Collections.Generic;
using System.Linq;

namespace MFramework.DLC
{
    public class MDirectedGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
    {
        //Tip����ߺͳ����Ƕ�Ӧ�ģ������Count=����Count
        //     ��Ϊ�����ʵ����Ϊ�����ŵģ����߾���Ϊ����ŵ�
        private IVertexEdgeDictionary<TVertex, TEdge> _vertexInEdges = new VertexEdgeDictionary<TVertex, TEdge>();
        private IVertexEdgeDictionary<TVertex, TEdge> _vertexOutEdges = new VertexEdgeDictionary<TVertex, TEdge>();

        public bool IsDirected => true;
        public bool AllowParallelEdges { get; }

        public bool IsVerticesEmpty => _vertexOutEdges.Count == 0;
        public bool IsEdgesEmpty => EdgeCount == 0;

        public int VertexCount => _vertexOutEdges.Count;
        public int EdgeCount { get; private set; }

        public MDirectedGraph() : this(false) { }
        public MDirectedGraph(bool allowParallelEdges)
        {
            AllowParallelEdges = allowParallelEdges;
        }

        public virtual IEnumerable<TVertex> Vertices => _vertexOutEdges.Keys.AsEnumerable();
        public virtual IEnumerable<TEdge> Edges => _vertexOutEdges.Values.SelectMany(edges => edges);

        public bool AddVertex(TVertex vertex)
        {
            if (!ContainsVertex(vertex))
            {
                _vertexOutEdges.Add(vertex, new EdgeList<TVertex, TEdge>());
                _vertexInEdges.Add(vertex, new EdgeList<TVertex, TEdge>());

                return true;
            }
            return false;
        }
        public bool RemoveVertex(TVertex vertex)
        {
            if (!ContainsVertex(vertex)) return false;

            //�Ƴ����㼴�Ƴ��������ر�
            RemoveInOutEdges(vertex);

            return true;
        }

        public bool AddEdge(TEdge edge)
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));
            if (!ContainsVertex(edge.Source))
            {
                AddVertex(edge.Source);
            }
            if (!ContainsVertex(edge.Target))
            {
                AddVertex(edge.Target);
            }

            //�������ƽ�бߣ����Ѿ����ڱߣ����½�
            if (!AllowParallelEdges && ContainsEdge(edge.Source, edge.Target))  
                return false;

            //���ڳ��ߣ�����������
            //������ߣ�������ǳ���
            _vertexOutEdges[edge.Source].Add(edge);
            _vertexInEdges[edge.Target].Add(edge);
            ++EdgeCount;

            return true;
        }
        public bool RemoveEdge(TEdge edge)
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));

            
            if (_vertexOutEdges.TryGetValue(edge.Source, out IEdgeList<TVertex, TEdge> outEdges)
                && outEdges.Remove(edge))//�Ƴ�Source�е�Edge
            {
                _vertexInEdges[edge.Target].Remove(edge);//�Ƴ�Target�е�Edge
                --EdgeCount;
                if (EdgeCount < 0) throw new Exception();

                return true;
            }

            return false;
        }
        private bool RemoveInOutEdges(TVertex vertex)
        {
            //�磺1->2->3 ɾ��2

            //���ҵ�2|->|3��ɾ��2��3�еı�
            IEdgeList<TVertex, TEdge> outEdges = _vertexOutEdges[vertex];
            _vertexOutEdges.Remove(vertex);//�������ʽɾ����
            foreach (TEdge outEdge in outEdges)
            {
                //ͬʱɾ����Ӧ����ı�
                _vertexInEdges[outEdge.Target].Remove(outEdge);
            }

            //���ҵ�1|->|2��ɾ��1��2�еı�
            IEdgeList<TVertex, TEdge> inEdges = _vertexInEdges[vertex];
            _vertexInEdges.Remove(vertex);//�Գ�����ʽɾ����
            foreach (TEdge inEdge in inEdges)
            {
                //ͬʱɾ����Ӧ���ı�
                _vertexOutEdges[inEdge.Source].Remove(inEdge);
            }

            EdgeCount -= outEdges.Count + inEdges.Count;
            if (EdgeCount < 0) throw new Exception();

            return true;
        }

        public bool TryGetOutEdges(TVertex vertex, out IEnumerable<TEdge> edges)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            if (_vertexOutEdges.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> outEdges))
            {
                edges = outEdges.AsEnumerable();
                return true;
            }

            edges = null;
            return false;
        }
        public bool TryGetInEdges(TVertex vertex, out IEnumerable<TEdge> edges)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            if (_vertexInEdges.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> inEdges))
            {
                edges = inEdges.AsEnumerable();
                return true;
            }

            edges = null;
            return false;
        }
        public bool TryGetEdges(TVertex source, TVertex target, out IEnumerable<TEdge> edges)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));

            if (_vertexOutEdges.TryGetValue(source, out IEdgeList<TVertex, TEdge> outEdges))
            {
                edges = outEdges.Where(edge => EqualityComparer<TVertex>.Default.Equals(edge.Target, target));
                return true;
            }

            edges = null;
            return false;
        }

        public int GetDegree(TVertex vertex)
        {
            return GetOutDegree(vertex) + GetInDegree(vertex);
        }
        public int GetOutDegree(TVertex vertex)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            if (_vertexOutEdges.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> outEdges))
                return outEdges.Count;

            throw new Exception();
        }
        public int GetInDegree(TVertex vertex)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            if (_vertexInEdges.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> inEdges))
                return inEdges.Count;
            throw new Exception();
        }

        public bool ContainsVertex(TVertex vertex)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            return _vertexOutEdges.ContainsKey(vertex);
        }
        public bool ContainsEdge(TEdge edge)
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));

            //��������ͼ��˵��
            //�ڳ�������edge.Source��Ϊ��ʼ�ڵ�(��ž���edge.Source<--->outEdges<--->edge.Target)
            bool flag1 = _vertexOutEdges.TryGetValue(edge.Source, out IEdgeList<TVertex, TEdge> outEdges);
            bool flag2 = outEdges.Contains(edge);

            return flag1 && flag2;
        }
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            //��ȡ���ĳ��ߣ�������û��target
            if (TryGetOutEdges(source, out IEnumerable<TEdge> outEdges))
                return outEdges.Any(edge => EqualityComparer<TVertex>.Default.Equals(edge.Target, target));
            return false;
        }
    }
}
