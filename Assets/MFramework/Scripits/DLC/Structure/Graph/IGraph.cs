namespace MFramework.DLC
{
    public interface IGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
    {
        bool IsDirected { get; }//�Ƿ�����
        bool AllowParallelEdges { get; }//�Ƿ�֧��ƽ�б�(��A<->B��2����)
    }
}
