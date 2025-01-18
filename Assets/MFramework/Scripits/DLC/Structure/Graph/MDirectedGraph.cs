using System;
using System.Collections.Generic;
using System.Linq;

namespace MFramework.DLC
{
    public class MDirectedGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
    {
        //Tip：入边和出边是对应的，即入边Count=出边Count
        //     因为入边其实就是为出点存放的，出边就是为入点存放的
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

            //移除顶点即移除顶点的相关边
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

            //如果允许平行边，但已经存在边，则不新建
            if (!AllowParallelEdges && ContainsEdge(edge.Source, edge.Target))  
                return false;

            //对于出边，放入的是入点
            //对于入边，放入的是出点
            _vertexOutEdges[edge.Source].Add(edge);
            _vertexInEdges[edge.Target].Add(edge);
            ++EdgeCount;

            return true;
        }
        public bool RemoveEdge(TEdge edge)
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));

            
            if (_vertexOutEdges.TryGetValue(edge.Source, out IEdgeList<TVertex, TEdge> outEdges)
                && outEdges.Remove(edge))//移除Source中的Edge
            {
                _vertexInEdges[edge.Target].Remove(edge);//移除Target中的Edge
                --EdgeCount;
                if (EdgeCount < 0) throw new Exception();

                return true;
            }

            return false;
        }
        private bool RemoveInOutEdges(TVertex vertex)
        {
            //如：1->2->3 删除2

            //即找到2|->|3，删除2和3中的边
            IEdgeList<TVertex, TEdge> outEdges = _vertexOutEdges[vertex];
            _vertexOutEdges.Remove(vertex);//以入点形式删除边
            foreach (TEdge outEdge in outEdges)
            {
                //同时删除对应出点的边
                _vertexInEdges[outEdge.Target].Remove(outEdge);
            }

            //即找到1|->|2，删除1和2中的边
            IEdgeList<TVertex, TEdge> inEdges = _vertexInEdges[vertex];
            _vertexInEdges.Remove(vertex);//以出点形式删除边
            foreach (TEdge inEdge in inEdges)
            {
                //同时删除对应入点的边
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

            //对于有向图来说，
            //在出边中找edge.Source则为起始节点(大概就是edge.Source<--->outEdges<--->edge.Target)
            bool flag1 = _vertexOutEdges.TryGetValue(edge.Source, out IEdgeList<TVertex, TEdge> outEdges);
            bool flag2 = outEdges.Contains(edge);

            return flag1 && flag2;
        }
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            //获取入点的出边，看看有没有target
            if (TryGetOutEdges(source, out IEnumerable<TEdge> outEdges))
                return outEdges.Any(edge => EqualityComparer<TVertex>.Default.Equals(edge.Target, target));
            return false;
        }
    }
}
