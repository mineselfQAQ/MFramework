using MFramework;
using MFramework.DLC;
using System;
using System.Collections;

namespace MFramework.DLC
{
    public class MBinarySearchTreeNode
    {
        #region Internal Field
        internal MBinarySearchTree list;

        internal object item;

        internal MBinarySearchTreeNode parent;

        internal MBinarySearchTreeNode left;

        internal MBinarySearchTreeNode right;
        #endregion

        #region Public Field
        public MBinarySearchTree List => list;

        public object Value
        {
            get
            {
                return item;
            }
            set
            {
                item = value;
            }
        }

        public MBinarySearchTreeNode Parent => parent;

        public MBinarySearchTreeNode Left => left;

        public MBinarySearchTreeNode Right => right;
        #endregion

        #region Constructor
        public MBinarySearchTreeNode(object value)
        {
            item = value;
        }

        internal MBinarySearchTreeNode(object value, MBinarySearchTree list)
        {
            this.list = list;
            item = value;
        }
        #endregion

        #region Function
        //使节点无效(没有内容与其产生联系，可GC回收)
        internal void Invalidate()
        {
            list = null;//核心---list清空将通不过ValidateNode()，从而无法进行任何操作
            parent = null;
            left = null;
            right = null;
        }
        #endregion
    }

    public class MBinarySearchTree : IEnumerable
    {
        #region Internal Field
        private MBinarySearchTreeNode root;

        private int count;
        #endregion

        #region Public Field
        public MBinarySearchTreeNode Root => root;

        public int Count => count;
        #endregion

        #region Constructor
        public MBinarySearchTree(){ }
        #endregion

        #region Function
        public void Add(object o)
        {
            MBinarySearchTreeNode newNode = new MBinarySearchTreeNode(o, this);

            InternalAdd(newNode);
        }
        public void Add(MBinarySearchTreeNode newNode)
        {
            ValidateNewNode(newNode);
            newNode.list = this;

            InternalAdd(newNode);
        }

        public void Remove(object o)
        {
            MBinarySearchTreeNode node = Search(o);

            InternalRemove(node);
        }
        public void Remove(MBinarySearchTreeNode node)
        {
            ValidateNode(node);

            InternalRemove(node);
        }

        public bool Contains(object o)
        {
            return Search(o) != null;
        }

        public object[] Sort()
        {
            return InOrder();
        }

        private void InternalAdd(MBinarySearchTreeNode newNode)
        {
            if (root == null)
            {
                root = newNode;
                count++;
                return;
            }

            IComparable num = newNode.item as IComparable;
            if (num == null) throw new Exception();//对于没有IComparable的数据不能比较

            MBinarySearchTreeNode cur = root;

            while (true)
            {
                if (num.CompareTo(cur.item) > 0)//num > cur.item
                {
                    if (cur.right == null)
                    {
                        newNode.parent = cur;
                        cur.right = newNode;
                        count++;
                        return;
                    }

                    cur = cur.right;//传入值更大，向右走
                }
                else if (num.CompareTo(cur.item) < 0)//num < cur.item
                {
                    if (cur.left == null)
                    {
                        newNode.parent = cur;
                        cur.left = newNode;
                        count++;
                        return;
                    }

                    cur = cur.left;//传入值更小，向左走
                }
                else
                {
                    throw new Exception();//一颗二叉搜索树中不可能有相同数字
                }
            }
        }
        private void InternalRemove(MBinarySearchTreeNode node)
        {
            //节点为空说明没有删除对象
            if (node == null)
            {
                throw new Exception();
            }

            //度为0(为叶子节点)
            if (node.left == null && node.right == null)
            {
                InvalidNode(node);
            }
            //度为1
            else if (node.left == null || node.right == null)
            {
                MBinarySearchTreeNode childNode = node.left ?? node.right;

                if (node == root)
                {
                    root = childNode;
                }
                else
                {
                    if (node.parent.left == node)//该节点为左节点
                    {
                        node.parent.left = childNode;
                    }
                    else if (node.parent.right == node)//该节点为右节点
                    {
                        node.parent.right = childNode;
                    }
                }
                node.Invalidate();//父节点已经链接其它节点，不需要使用InvalidNode()(对于根节点也不用做)
            }
            //度为2
            else
            {
                MBinarySearchTreeNode tempNode = node.right;
                while (tempNode.left != null)
                {
                    tempNode = tempNode.left;
                }
                node.item = tempNode.item;
                InvalidNode(tempNode);
            }

            count--;
        }

        private MBinarySearchTreeNode Search(object o)
        {
            IComparable num = o as IComparable;
            if (num == null) throw new Exception();//对于没有IComparable的数据不能比较

            MBinarySearchTreeNode cur = root;

            while (cur != null)
            {
                if (num.CompareTo(cur.item) > 0)//num > cur.item
                {
                    cur = cur.right;//传入值更大，向右走
                }
                else if (num.CompareTo(cur.item) < 0)//num < cur.item
                {
                    cur = cur.left;//传入值更小，向左走
                }
                else
                {
                    break;//就是该值，获取
                }
            }
            return cur;
        }

        private void ValidateNode(MBinarySearchTreeNode node)
        {
            //节点必须存在
            if (node == null)
            {
                throw new Exception();
            }
            //节点所述链表应该为同一个
            if (node.list != this)
            {
                throw new Exception();
            }
        }
        private void ValidateNewNode(MBinarySearchTreeNode node)
        {
            //节点必须存在
            if (node == null)
            {
                throw new Exception();
            }
            //新节点的list应该为null(在外部创建的node的list是null的)
            if (node.list != null)
            {
                throw new Exception();
            }
        }
        private void InvalidNode(MBinarySearchTreeNode node)
        {
            //除根节点外，一般节点都需要将父节点与该节点断开链接
            if (node != root)
            {
                MBinarySearchTreeNode parentNode = node.parent;

                //根据节点在父节点的左右断开
                if (parentNode.left == node)
                {
                    parentNode.left = null;
                }
                else if (parentNode.right == node)
                {
                    parentNode.right = null;
                }
            }

            //断开自己的引用
            node.Invalidate();
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var item in InOrder())
            {
                yield return item;
            }
        }
        private object[] InOrder()
        {
            ValidateNode(root);

            object[] list = new object[count];
            InternalInOrder(root, list, 0);

            return list;
        }
        private int InternalInOrder(MBinarySearchTreeNode root, object[] list, int index)
        {
            if (root == null) return index - 1;
            int next = InternalInOrder(root.left, list, index);
            next++;
            list[next] = root.item;
            return InternalInOrder(root.right, list, next + 1);
        }

        #endregion
    }

    public static class MBinarySearchTreeExtension
    {
        public static void Print(this MBinarySearchTree tree)
        {
            MQueue queue = new MQueue();
            queue.Enqueue(tree.Root);

            int count = 0;
            int beforeCount = 0;
            int level = 0;
            string levelStr = "";
            bool isNotBottom = true;
            while (queue.Count != 0)
            {
                MBinarySearchTreeNode node = (MBinarySearchTreeNode)queue.Dequeue();
                count++;

                //如果计数翻倍，就换行
                if (count == 1 || count == beforeCount * 2)
                {
                    //如果没有下一行(为底部的下一行)，就结束
                    if (!isNotBottom) break;
                    isNotBottom = false;

                    beforeCount = count;
                    level++;

                    levelStr += $"\n层{level}: ";
                }
                //输出
                if (node != null)
                {
                    levelStr += $"{node.Value} ";
                }
                else
                {
                    levelStr += "X ";
                }

                //无论如何，都将node填入(但是node不能为空，也就是说该节点不是由空节点派生出来的)
                MBinarySearchTreeNode leftNode = node == null ? null : node.left;
                MBinarySearchTreeNode rightNode = node == null ? null : node.right;
                queue.Enqueue(leftNode);
                queue.Enqueue(rightNode);

                //只要有一层中有一个子节点，就说明不是最底层
                if (leftNode != null || rightNode != null)
                {
                    isNotBottom = true;
                }
            }
            MLog.Print(MLog.Color("---二叉树可视化---", UnityEngine.Color.black, true, false) + levelStr);
        }

        public static void SortPrint(this MBinarySearchTree tree)
        {
            MLog.Print("输出: ");

            if (tree.Count == 0)
            {
                MLog.Print("无元素");
                return;
            }

            string outputStr = "";
            foreach (var item in tree)
            {
                outputStr += $"{item} ";
            }
            MLog.Print(outputStr);
        }
    }
}