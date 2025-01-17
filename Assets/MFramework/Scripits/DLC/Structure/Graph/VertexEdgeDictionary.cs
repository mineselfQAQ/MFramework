using System.Collections.Generic;

namespace MFramework.DLC
{
    public interface IVertexEdgeDictionary<TVertex, TEdge> : 
        IDictionary<TVertex, IEdgeList<TVertex, TEdge>> where TEdge : IEdge<TVertex> { }

    public class VertexEdgeDictionary<TVertex, TEdge> : 
        Dictionary<TVertex, IEdgeList<TVertex, TEdge>>, IVertexEdgeDictionary<TVertex, TEdge> where TEdge : IEdge<TVertex>
    {
        public VertexEdgeDictionary() { }
        public VertexEdgeDictionary(int capacity) : base(capacity) { }
    }
}
