using System;
using System.Collections.Generic;
using System.Linq;

namespace MFramework.DLC
{
    public class MDirectedGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
    {
        private IVertexEdgeDictionary<TVertex, TEdge> _vertexInEdges = new VertexEdgeDictionary<TVertex, TEdge>();
        private IVertexEdgeDictionary<TVertex, TEdge> _vertexOutEdges = new VertexEdgeDictionary<TVertex, TEdge>();

        public bool IsDirected => true;
        public bool AllowParallelEdges { get; }

        public MDirectedGraph() : this(false) { }
        public MDirectedGraph(bool allowParallelEdges)
        {
            AllowParallelEdges = allowParallelEdges;
        }

        public bool IsVerticesEmpty => _vertexOutEdges.Count == 0;
        public bool IsEdgesEmpty => EdgeCount == 0;

        public int VertexCount => _vertexOutEdges.Count;
        public int EdgeCount { get; private set; }

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
            return false;
        }

        public bool AddEdge(TEdge edge)
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));
            if (!ContainsVertex(edge.Source) || !ContainsVertex(edge.Target)) throw new Exception();

            //如果允许平行边，但已经存在边，则不新建
            if (!AllowParallelEdges && ContainsEdge(edge.Source, edge.Target))  
                return false;

            _vertexOutEdges[edge.Source].Add(edge);
            _vertexInEdges[edge.Target].Add(edge);
            ++EdgeCount;

            return true;
        }

        private bool RemoveInOutEdges(TVertex vertex)
        {
            IEdgeList<TVertex, TEdge> outEdges = _vertexOutEdges[vertex];
            _vertexOutEdges.Remove(vertex);//删除以顶点起始的所有边

            foreach (TEdge outEdge in outEdges)
            {
                _vertexInEdges[outEdge.Target].Remove(outEdge);
            }

            IEdgeList<TVertex, TEdge> inEdges = _vertexInEdges[vertex];
            _vertexInEdges.Remove(vertex);//删除
            foreach (TEdge inEdge in inEdges)
            {
                _vertexOutEdges[inEdge.Source].Remove(inEdge);
            }

            EdgeCount -= outEdges.Count + inEdges.Count;
            if (EdgeCount < 0) throw new Exception();

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
            return false;
        }
    }
}
