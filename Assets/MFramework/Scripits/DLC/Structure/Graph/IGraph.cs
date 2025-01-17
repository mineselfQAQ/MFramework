namespace MFramework.DLC
{
    public interface IGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
    {
        bool IsDirected { get; }//是否有向
        bool AllowParallelEdges { get; }//是否支持平行边(如A<->B有2条边)
    }
}
