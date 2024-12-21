using MFramework;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MFramework.DLC
{
    public class MBinaryTreeNode
    {
        #region Internal Field
        internal MBinaryTree list;

        internal object item;

        internal MBinaryTreeNode parent;

        internal MBinaryTreeNode left;

        internal MBinaryTreeNode right;
        #endregion

        #region Public Field
        public MBinaryTree List => list;

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

        public MBinaryTreeNode Parent => parent;

        public MBinaryTreeNode Left => left;

        public MBinaryTreeNode Right => right;
        #endregion

        #region Constructor
        public MBinaryTreeNode(object value)
        {
            item = value;
        }

        internal MBinaryTreeNode(object value, MBinaryTree list)
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

    public class MBinaryTree
    {
        #region Internal Field
        private MBinaryTreeNode root;

        private int count;
        #endregion

        #region Public Field
        public MBinaryTreeNode Root => root;

        public int Count => count;
        #endregion

        #region Constructor
        public MBinaryTree(object value) 
        {
            MBinaryTreeNode root = new MBinaryTreeNode(value, this);
            this.root = root;
            count++;
        }
        public MBinaryTree(MBinaryTreeNode root)
        {
            ValidateNewNode(root);
            this.root = root;
            count++;
        }
        #endregion

        #region Function
        public MBinaryTreeNode AddLeft(MBinaryTreeNode node, object value)
        {
            ValidateNode(node);

            MBinaryTreeNode newNode = new MBinaryTreeNode(value, this);

            InternalAddLeft(node, newNode);

            return newNode;
        }
        public void AddLeft(MBinaryTreeNode node, MBinaryTreeNode newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);

            InternalAddLeft(node, newNode);
            newNode.list = this;
        }

        public MBinaryTreeNode AddRight(MBinaryTreeNode node, object value)
        {
            ValidateNode(node);

            MBinaryTreeNode newNode = new MBinaryTreeNode(value, this);

            InternalAddRight(node, newNode);

            return newNode;
        }
        public void AddRight(MBinaryTreeNode node, MBinaryTreeNode newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);

            InternalAddRight(node, newNode);
            newNode.list = this;
        }

        public bool Remove(MBinaryTreeNode node)
        {
            ValidateNode(node);//移除之前，该节点肯定是有效(存在)的
            InternalRemove(node);

            return true;
        }

        public object[] LevelOrder()
        {
            object[] list = new object[count];

            MQueue queue = new MQueue();
            queue.Enqueue(root);

            int i = 0;
            while (queue.Count != 0)
            {
                MBinaryTreeNode node = (MBinaryTreeNode)queue.Dequeue();

                list[i++] = node.item;//将值存入数组中

                //只要有子节点，就以先左再右的顺序添加node
                if (node.left != null)
                {
                    queue.Enqueue(node.left);
                }
                if (node.right != null)
                {
                    queue.Enqueue(node.right);
                }
            }

            return list;
        }

        public object[] PreOrder()
        {
            ValidateNode(root);

            object[] list = new object[count];
            InternalPreOrder(root, list, 0);

            return list;
        }
        public object[] InOrder()
        {
            ValidateNode(root);

            object[] list = new object[count];
            InternalInOrder(root, list, 0);

            return list;
        }
        public object[] PostOrder()
        {
            ValidateNode(root);

            object[] list = new object[count];
            InternalPostOrder(root, list, 0);

            return list;
        }

        public bool Contains(object o)
        {
            object[] list = PreOrder();
            foreach (var item in list)
            {
                if (o == null)
                {
                    if (item == null)
                    {
                        return true;
                    }
                }
                else if (item != null && item.Equals(o))
                {
                    return true;
                }
            }
            return false;
        }

        private void ValidateNode(MBinaryTreeNode node)
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
        private void ValidateNewNode(MBinaryTreeNode node)
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
        private void InvalidNode(MBinaryTreeNode node)
        {
            //除根节点外，一般节点都需要将父节点与该节点断开链接
            if (node != root)
            {
                MBinaryTreeNode parentNode = node.parent;

                //根据节点在父节点的位置断开
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

        //Tip:完整名称应该为InternalAddLeftAfter()，为**后插**
        //在树中，前插不是一个非常合理的操作，不进行实现
        private void InternalAddLeft(MBinaryTreeNode node, MBinaryTreeNode newNode)
        {
            newNode.parent = node;

            //如果节点为叶子节点，为链表的AddLast()
            if (node.left == null && node.right == null)
            {
                node.left = newNode;
                count++;
                return;
            }

            //一般情况为插入
            newNode.left = node.left;
            node.left = newNode;
            count++;
        }
        private void InternalAddRight(MBinaryTreeNode node, MBinaryTreeNode newNode)
        {
            newNode.parent = node;

            //如果节点为叶子节点，为链表的AddLast()
            if (node.left == null && node.right == null)
            {
                node.right = newNode;
                count++;
                return;
            }

            //一般情况为插入
            newNode.right = node.right;
            node.right = newNode;
            count++;
        }
        private void InternalRemove(MBinaryTreeNode node)
        {
            MQueue queue = new MQueue();
            queue.Enqueue(node);

            //BFS，不断向下断开链接
            while (queue.Count != 0)
            {
                MBinaryTreeNode tempNode = (MBinaryTreeNode)queue.Dequeue();
                count--;

                //只要有子节点，就以先左再右的顺序添加node
                if (tempNode.left != null)
                {
                    queue.Enqueue(tempNode.left);
                }
                if (tempNode.right != null)
                {
                    queue.Enqueue(tempNode.right);
                }

                InvalidNode(tempNode);//断开链接
            }
        }

        private int InternalPreOrder(MBinaryTreeNode root, object[] list, int index)
        {
            if (root == null) return index - 1;
            list[index] = root.item;
            int next = InternalPreOrder(root.left, list, index + 1);
            return InternalPreOrder(root.right, list, next + 1);
        }
        private int InternalInOrder(MBinaryTreeNode root, object[] list, int index)
        {
            if (root == null) return index - 1;
            int next = InternalInOrder(root.left, list, index);
            next++;
            list[next] = root.item;
            return InternalInOrder(root.right, list, next + 1);
        }
        private int InternalPostOrder(MBinaryTreeNode root, object[] list, int index)
        {
            if (root == null) return index - 1;
            int next = InternalPostOrder(root.left, list, index);
            next++;
            next = InternalPostOrder(root.right, list, next);
            next++;
            list[next] = root.item;
            return next;
        }

        #endregion
    }

    public static class MBinaryTreeExtension
    {
        public static void Print(this MBinaryTree tree)
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
                MBinaryTreeNode node = (MBinaryTreeNode)queue.Dequeue();
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
                MBinaryTreeNode leftNode = node == null ? null : node.left;
                MBinaryTreeNode rightNode = node == null ? null : node.right;
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
    }
}