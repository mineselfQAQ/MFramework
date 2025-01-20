using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace MFramework.DLC
{
    public class MDirectedGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
    {
        //Tip����ߺͳ����Ƕ�Ӧ�ģ������Count=����Count
        //     ��Ϊ�����ʵ����Ϊ�����ŵģ����߾���Ϊ����ŵ�
        private IVertexEdgeDictionary<TVertex, TEdge> _inDic = new VertexEdgeDictionary<TVertex, TEdge>();
        private IVertexEdgeDictionary<TVertex, TEdge> _outDic = new VertexEdgeDictionary<TVertex, TEdge>();

        public bool IsDirected => true;
        public bool AllowParallelEdges { get; }

        public bool IsVerticesEmpty => _outDic.Count == 0;
        public bool IsEdgesEmpty => EdgeCount == 0;

        public int VertexCount => _outDic.Count;
        public int EdgeCount { get; private set; }

        public MDirectedGraph() : this(false) { }
        public MDirectedGraph(bool allowParallelEdges)
        {
            AllowParallelEdges = allowParallelEdges;
        }

        //������inDic����outDic��ֻҪ����˶��㣬�ʹ��ڼ�ֵ��
        public virtual IEnumerable<TVertex> Vertices => _outDic.Keys.AsEnumerable();
        public virtual IEnumerable<TEdge> Edges => _outDic.Values.SelectMany(edges => edges);

        #region �ṹ��������
        public bool AddVertex(TVertex vertex)
        {
            if (!ContainsVertex(vertex))
            {
                _outDic.Add(vertex, new EdgeList<TVertex, TEdge>());
                _inDic.Add(vertex, new EdgeList<TVertex, TEdge>());

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
            _outDic[edge.Source].Add(edge);
            _inDic[edge.Target].Add(edge);
            ++EdgeCount;

            return true;
        }
        public bool RemoveEdge(TEdge edge)
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));

            
            if (_outDic.TryGetValue(edge.Source, out IEdgeList<TVertex, TEdge> outEdges)
                && outEdges.Remove(edge))//�Ƴ�Source�е�Edge
            {
                _inDic[edge.Target].Remove(edge);//�Ƴ�Target�е�Edge
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
            IEdgeList<TVertex, TEdge> outEdges = _outDic[vertex];
            _outDic.Remove(vertex);//�������ʽɾ����
            foreach (TEdge outEdge in outEdges)
            {
                //ͬʱɾ����Ӧ����ı�
                _inDic[outEdge.Target].Remove(outEdge);
            }

            //���ҵ�1|->|2��ɾ��1��2�еı�
            IEdgeList<TVertex, TEdge> inEdges = _inDic[vertex];
            _inDic.Remove(vertex);//�Գ�����ʽɾ����
            foreach (TEdge inEdge in inEdges)
            {
                //ͬʱɾ����Ӧ���ı�
                _outDic[inEdge.Source].Remove(inEdge);
            }

            EdgeCount -= outEdges.Count + inEdges.Count;
            if (EdgeCount < 0) throw new Exception();

            return true;
        }

        public bool TryGetOutEdges(TVertex vertex, out IEnumerable<TEdge> edges)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            if (_outDic.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> outEdges))
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

            if (_inDic.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> inEdges))
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

            if (_outDic.TryGetValue(source, out IEdgeList<TVertex, TEdge> outEdges))
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

            if (_outDic.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> outEdges))
                return outEdges.Count;

            throw new Exception();
        }
        public int GetInDegree(TVertex vertex)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            if (_inDic.TryGetValue(vertex, out IEdgeList<TVertex, TEdge> inEdges))
                return inEdges.Count;
            throw new Exception();
        }

        public bool ContainsVertex(TVertex vertex)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));

            return _outDic.ContainsKey(vertex);
        }
        public bool ContainsEdge(TEdge edge)
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));

            //��������ͼ��˵��
            //�ڳ�������edge.Source��Ϊ��ʼ�ڵ�(��ž���edge.Source<--->outEdges<--->edge.Target)
            bool flag1 = _outDic.TryGetValue(edge.Source, out IEdgeList<TVertex, TEdge> outEdges);
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

                //�ҳ���(����ֵ���Ȼ�У����ǲ�Ӧ���ܷ���)
                foreach (var edge in _outDic[vertex])
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

            foreach (var edge in _outDic[vertex])
            {
                DFSInternal(edge.Target, res, visited);
            }
        }

        public bool HasCycle()
        {
            HashSet<TVertex> visited = new HashSet<TVertex>();
            HashSet<TVertex> pathSet = new HashSet<TVertex>();

            foreach (var vertex in Vertices)
            {
                if (HasCycleDFS(vertex, visited, pathSet))
                    return true;
            }

            return false;
        }
        private bool HasCycleDFS(TVertex vertex, HashSet<TVertex> visited, HashSet<TVertex> pathSet)
        {
            if (pathSet.Contains(vertex)) return true;//�Ѵ���˵�������˻�

            if (visited.Contains(vertex)) return false;//���ʹ��ͷ���

            visited.Add(vertex);
            pathSet.Add(vertex);

            foreach (var edge in _outDic[vertex])
            {
                if (HasCycleDFS(edge.Target, visited, pathSet)) return true;
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

            //�������ж���Ѱ�һ�
            bool isFirstVertex = true;
            foreach (var vertex in Vertices)
            {
                bool isFirstTime = true;
                if (!visited.Contains(vertex)) isFirstVertex = true;//�ҵ���һ������
                FindCycleDFS(vertex, visited, pathList, allCycles, isFirstVertex, isFirstTime);
                isFirstVertex = false;
            }

            return allCycles;
        }
        private void FindCycleDFS(TVertex vertex, HashSet<TVertex> visitedVertex, List<TVertex> pathList, List<List<TVertex>> allCycles, bool isFirstVertex, bool isFirstTime)
        {
            //·���г�����ͬ��˵�����ֻ�
            //�磺A->B->C->A������Aʱ˵��������
            if (pathList.Contains(vertex))
            {
                //����·������ΪA->B->C->D->B��������Ҫ�ҵ�����ͷ
                var cycleStartIndex = pathList.IndexOf(vertex);
                var cycle = new List<TVertex>(pathList.GetRange(cycleStartIndex, pathList.Count - cycleStartIndex));

                cycle.Add(vertex);//���ͷβԪ��(��A->B->C->A)
                allCycles.Add(cycle);

                return;
            }

            //���������
            pathList.Add(vertex);
            if(!visitedVertex.Contains(vertex)) visitedVertex.Add(vertex);

            //�������������Ӷ���ݹ����
            foreach (var edge in _outDic[vertex])
            {
                var next = edge.Target;

                //�ж��Ƿ����ѷ���·��
                bool visited = false;
                if (!isFirstVertex && isFirstTime)//��һ����ȫ�������������ų� && ��һ����Ҫ�жϣ�������ɹ���˵������·�� 
                {
                    foreach (var cycle in allCycles)
                    {
                        //�鿴��һ�����Ƿ��Ѿ�����
                        //�磺A->B->C->A����ʱ���B����һ�β�ѯ����������B->C��˵�����Ѿ���¼�����
                        if (EqualityComparer<TVertex>.Default.Equals(cycle[cycle.IndexOf(vertex) + 1], next))
                        {
                            visited = true;
                            break;
                        }
                    }
                }

                if (!visited) FindCycleDFS(next, visitedVertex, pathList, allCycles, isFirstVertex, isFirstTime);
            }

            isFirstTime = false;
            pathList.RemoveAt(pathList.Count - 1);//���ֽ�����·������
        }
        #endregion
    }
}
