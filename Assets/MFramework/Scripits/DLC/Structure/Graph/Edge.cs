using System;
using System.Collections.Generic;

namespace MFramework.DLC
{
    public interface IEdge<out TVertex>
    {
        TVertex Source { get; }
        TVertex Target { get; }
    }
    public interface IUndirectedEdge<out TVertex> : IEdge<TVertex> { }//һ��֤��(�������)

    public interface IEdgeList<TVertex, TEdge> : IList<TEdge>, ICloneable where TEdge : IEdge<TVertex>
    {
        new IEdgeList<TVertex, TEdge> Clone();
    }

    public class Edge<TVertex> : IEdge<TVertex>
    {
        public TVertex Source { get; }//from
        public TVertex Target { get; }//to

        public Edge(TVertex source, TVertex target)
        {
            Source = source;
            Target = target;
        }

        //˫��
        public override string ToString()
        {
            return $"{Source} -> {Target}";
        }
    }

    public class UndirectedEdge<TVertex> : IUndirectedEdge<TVertex>, IEdge<TVertex>
    {
        //˵��SourceTarget������������ͼ����ʵû��
        public TVertex Source { get; }
        public TVertex Target { get; }

        public UndirectedEdge(TVertex source, TVertex target)
        {
            Source = source;
            Target = target;
        }

        //˫��
        public override string ToString()
        {
            return $"{Source} <-> {Target}";
        }
    }

    /// <summary>
    /// �����ıߣ�ָ����ĳ���������ı�
    /// </summary>
    public class EdgeList<TVertex, TEdge> :
        List<TEdge>, IEdgeList<TVertex, TEdge> where TEdge : IEdge<TVertex>
    {
        public EdgeList() { }
        public EdgeList(int capacity) : base(capacity) { }
        public EdgeList(EdgeList<TVertex, TEdge> other) : base(other) { }

        public EdgeList<TVertex, TEdge> Clone()
        {
            return new EdgeList<TVertex, TEdge>(this);
        }

        //��¡����
        IEdgeList<TVertex, TEdge> IEdgeList<TVertex, TEdge>.Clone()
        {
            return Clone();
        }
        object ICloneable.Clone()
        {
            return Clone();
        }
    }

    public static class EdgeExtension
    {
        public static bool IsSelfEdge<TVertex>(this IEdge<TVertex> edge)
        {
            if (edge is null) throw new ArgumentNullException(nameof(edge));

            return EqualityComparer<TVertex>.Default.Equals(edge.Source, edge.Target);
        }
    }
}
