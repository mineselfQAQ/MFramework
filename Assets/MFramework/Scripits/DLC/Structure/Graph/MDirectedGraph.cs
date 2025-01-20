using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace MFramework.DLC
{
    public class MDirectedGraph<TVertex, TEdge> where TEdge : IEdge<TVertex>
    {
        //Tip：入边和出边是对应的，即入边Count=出边Count
        //     因为入边其实就是为出点存放的，出边就是为入点存放的
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

        //无论是inDic还是outDic，只要添加了顶点，就存在键值对
        public virtual IEnumerable<TVertex> Vertices => _outDic.Keys.AsEnumerable();
        public virtual IEnumerable<TEdge> Edges => _outDic.Values.SelectMany(edges => edges);

        #region 结构基础方法
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
            _outDic[edge.Source].Add(edge);
            _inDic[edge.Target].Add(edge);
            ++EdgeCount;

            return true;
        }
        public bool RemoveEdge(TEdge edge)
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));

            
            if (_outDic.TryGetValue(edge.Source, out IEdgeList<TVertex, TEdge> outEdges)
                && outEdges.Remove(edge))//移除Source中的Edge
            {
                _inDic[edge.Target].Remove(edge);//移除Target中的Edge
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
            IEdgeList<TVertex, TEdge> outEdges = _outDic[vertex];
            _outDic.Remove(vertex);//以入点形式删除边
            foreach (TEdge outEdge in outEdges)
            {
                //同时删除对应出点的边
                _inDic[outEdge.Target].Remove(outEdge);
            }

            //即找到1|->|2，删除1和2中的边
            IEdgeList<TVertex, TEdge> inEdges = _inDic[vertex];
            _inDic.Remove(vertex);//以出点形式删除边
            foreach (TEdge inEdge in inEdges)
            {
                //同时删除对应入点的边
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

            //对于有向图来说，
            //在出边中找edge.Source则为起始节点(大概就是edge.Source<--->outEdges<--->edge.Target)
            bool flag1 = _outDic.TryGetValue(edge.Source, out IEdgeList<TVertex, TEdge> outEdges);
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
        #endregion

        #region 算法
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

                //找出边(入边字典虽然有，但是不应该能访问)
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
            if (pathSet.Contains(vertex)) return true;//已存在说明出现了环

            if (visited.Contains(vertex)) return false;//访问过就返回

            visited.Add(vertex);
            pathSet.Add(vertex);

            foreach (var edge in _outDic[vertex])
            {
                if (HasCycleDFS(edge.Target, visited, pathSet)) return true;
            }

            pathSet.Remove(vertex);//顶点寻找结束就路径回退
            return false;
        }

        public List<List<TVertex>> FindCycle()
        {
            //已访问节点(无用)
            //如：先进行0->1->2->0检测，此时0/1/2都会加入visited，第二个环为0->1->3->4->2->0，遇到2就退出了
            //所以仅用于判断是否顶点为孤岛(也许有其它情况？但是没有访问过说明出现了新的可能)
            HashSet<TVertex> visited = new HashSet<TVertex>();
            //路径记录
            List<TVertex> pathList = new List<TVertex>();//Tip：如果只是判断是否存在，只需要使用HashSet即可
            //输出环列表
            List<List<TVertex>> allCycles = new List<List<TVertex>>();

            //遍历所有顶点寻找环
            bool isFirstVertex = true;
            foreach (var vertex in Vertices)
            {
                bool isFirstTime = true;
                if (!visited.Contains(vertex)) isFirstVertex = true;//找到另一个可能
                FindCycleDFS(vertex, visited, pathList, allCycles, isFirstVertex, isFirstTime);
                isFirstVertex = false;
            }

            return allCycles;
        }
        private void FindCycleDFS(TVertex vertex, HashSet<TVertex> visitedVertex, List<TVertex> pathList, List<List<TVertex>> allCycles, bool isFirstVertex, bool isFirstTime)
        {
            //路径中出现相同，说明出现环
            //如：A->B->C->A，出现A时说明环出现
            if (pathList.Contains(vertex))
            {
                //由于路径可能为A->B->C->D->B，所以需要找到环的头
                var cycleStartIndex = pathList.IndexOf(vertex);
                var cycle = new List<TVertex>(pathList.GetRange(cycleStartIndex, pathList.Count - cycleStartIndex));

                cycle.Add(vertex);//添加头尾元素(如A->B->C->A)
                allCycles.Add(cycle);

                return;
            }

            //访问则添加
            pathList.Add(vertex);
            if(!visitedVertex.Contains(vertex)) visitedVertex.Add(vertex);

            //遍历对所有连接顶点递归进入
            foreach (var edge in _outDic[vertex])
            {
                var next = edge.Target;

                //判断是否是已访问路径
                bool visited = false;
                if (!isFirstVertex && isFirstTime)//第一轮需全部搜索，不能排除 && 第一次需要判断，如果不成功才说明是新路径 
                {
                    foreach (var cycle in allCycles)
                    {
                        //查看下一顶点是否已经存在
                        //如：A->B->C->A，此时如果B进行一次查询操作，发现B->C则说明是已经记录的情况
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
            pathList.RemoveAt(pathList.Count - 1);//此轮结束，路径回退
        }
        #endregion
    }
}
