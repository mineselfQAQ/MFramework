using System;
using System.Collections;

namespace MFramework.DLC
{
    public class MAVLTreeNode
    {
        #region Internal Field
        internal MAVLTree list;

        internal object item;

        internal MAVLTreeNode parent;

        internal MAVLTreeNode left;

        internal MAVLTreeNode right;

        internal int height;
        #endregion

        #region Public Field
        public MAVLTree List => list;

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

        public MAVLTreeNode Parent => parent;

        public MAVLTreeNode Left => left;

        public MAVLTreeNode Right => right;
        #endregion

        #region Constructor
        public MAVLTreeNode(object value)
        {
            item = value;
        }

        internal MAVLTreeNode(object value, MAVLTree list)
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

    public class MAVLTree : IEnumerable
    {
        #region Internal Field
        private MAVLTreeNode root;

        private int count;
        #endregion

        #region Public Field
        public MAVLTreeNode Root => root;

        public int Count => count;
        #endregion

        #region Constructor
        public MAVLTree() { }
        #endregion

        #region Function
        public void Add(object o)
        {
            root = InternalAdd(root, o, null);
            count++;
        }

        public void Remove(object o)
        {
            if (root == null) throw new Exception();//root为空那么不可能有内容需要remove

            InternalRemove(root, o);
            count--;
        }

        public bool Contains(object o)
        {
            return Search(o) != null;
        }

        public object[] Sort()
        {
            return InOrder();
        }

        private MAVLTreeNode InternalAdd(MAVLTreeNode node, object o, MAVLTreeNode beforeNode)
        {
            if (node == null)
            {
                MAVLTreeNode newNode = new MAVLTreeNode(o, this);
                newNode.parent = beforeNode;
                return newNode;
            }

            IComparable num = o as IComparable;
            if (num == null) throw new Exception();//对于没有IComparable的数据不能比较
            
            if (num.CompareTo(node.item) < 0)//传入值更小，向左走
            {
                node.left = InternalAdd(node.left, o, node);
            }
            else if (num.CompareTo(node.item) > 0)//传入值更大，向右走
            {
                node.right = InternalAdd(node.right, o, node);
            }
            else
            {
                //搜索树插入必不可能相等
                throw new Exception();
            }

            UpdateHeight(node);
            node = Rotate(node);

            return node;
        }
        private MAVLTreeNode InternalRemove(MAVLTreeNode node, object o)
        {
            if (node == null) return null;

            IComparable num = o as IComparable;
            if (num == null) throw new Exception();//对于没有IComparable的数据不能比较

            if (num.CompareTo(node.item) < 0)//传入值更小，向左走
            {
                node.left = InternalRemove(node.left, o);
            }
            else if (num.CompareTo(node.item) > 0)//传入值更大，向右走
            {
                node.right = InternalRemove(node.right, o);
            }
            else
            {
                //度为0(为叶子节点)
                if (node.left == null && node.right == null)
                {
                    //叶子节点的null会返回给上一层，那么上一层的下一层会设置为null，不用InvalidNode()
                    node.Invalidate();
                    return null;
                }
                //度为1
                else if (node.left == null || node.right == null)
                {
                    MAVLTreeNode childNode = node.left ?? node.right;

                    //另一种写法
                    //node = childNode;
                    //注意：需要将节点Invalidate()，可以通过temp存储node在赋值后删除
                    //含义为：将node指向childNode的堆内存，那么childNode就被"提上来了"，
                    //而且node还是那个node，node.parent和node的关系没有发生改变

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
                    MAVLTreeNode tempNode = node.right;
                    while (tempNode.left != null)
                    {
                        tempNode = tempNode.left;
                    }
                    node.item = tempNode.item;
                    InvalidNode(tempNode);
                }
            }

            UpdateHeight(node);
            node = Rotate(node);

            return node;
        }

        private MAVLTreeNode Search(object o)
        {
            IComparable num = o as IComparable;
            if (num == null) throw new Exception();//对于没有IComparable的数据不能比较

            MAVLTreeNode cur = root;

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

        private void ValidateNode(MAVLTreeNode node)
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
        private void ValidateNewNode(MAVLTreeNode node)
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
        private void InvalidNode(MAVLTreeNode node)
        {
            //除根节点外，一般节点都需要将父节点与该节点断开链接
            if (node != root)
            {
                MAVLTreeNode parentNode = node.parent;

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

        private int GetHeight(MAVLTreeNode node) => node == null ? -1 : node.height;
        private void UpdateHeight(MAVLTreeNode node)
        {
            node.height = Math.Max(GetHeight(node.left), GetHeight(node.right)) + 1;
        }
        private int BalanceFactor(MAVLTreeNode node)
        {
            if (node == null) return 0;

            return GetHeight(node.left) - GetHeight(node.right);
        }

        //Tip:操作完成后还未与父节点链接
        private MAVLTreeNode RightRotate(MAVLTreeNode node)
        {
            MAVLTreeNode childNode = node.left;

            //理论上不会发生，但是如果左子节点都没找到，那么就不可能右旋
            if (childNode == null)
            {
                return node;
            }

            if (childNode.right == null)//childNode无子节点情况
            {
                childNode.parent = node.parent;
                childNode.right = node;
                node.left = null;
                node.parent = childNode;
            }
            else//childNode有子节点情况
            {
                MAVLTreeNode grandChildNode = childNode.right;
                childNode.parent = node.parent;
                childNode.right = node;
                node.left = grandChildNode;
                node.parent = childNode;
                grandChildNode.parent = node;
            }

            //node和childNode顺序发生改变，高度自然发生改变，而子节点不受影响
            UpdateHeight(node);
            UpdateHeight(childNode);

            return childNode;//为更新后的"根节点"
        }
        private MAVLTreeNode LeftRotate(MAVLTreeNode node)
        {
            MAVLTreeNode childNode = node.right;

            if (childNode == null)
            {
                return node;
            }

            if (childNode.left == null)//childNode无子节点情况
            {
                childNode.parent = node.parent;
                childNode.left = node;
                node.right = null;
                node.parent = childNode;
            }
            else//childNode有子节点情况
            {
                MAVLTreeNode grandChildNode = childNode.left;
                childNode.parent = node.parent;
                childNode.left = node;
                node.right = grandChildNode;
                node.parent = childNode;
                grandChildNode.parent = node;
            }

            //node和childNode顺序发生改变，高度自然发生改变，而子节点不受影响
            UpdateHeight(node);
            UpdateHeight(childNode);

            return childNode;//为更新后的"根节点"
        }
        private MAVLTreeNode Rotate(MAVLTreeNode node)
        {
            int balanceFactor = BalanceFactor(node);

            if (balanceFactor > 1)//左偏树情况
            {
                if (BalanceFactor(node.left) >= 0)//节点在左侧
                {
                    //右旋
                    return RightRotate(node);
                }
                else//节点在右侧
                {
                    //先左旋，再右旋
                    node.left = LeftRotate(node.left);
                    return RightRotate(node);
                }
            }
            if (balanceFactor < -1)//右偏树情况
            {
                if (BalanceFactor(node.right) <= 0)//节点在右侧
                {
                    //左旋
                    return LeftRotate(node);
                }
                else//节点在左侧
                {
                    //先右旋，再左旋
                    node.right = RightRotate(node.right);
                    return LeftRotate(node);
                }
            }
            //没有失衡，直接返回
            return node;
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
        private int InternalInOrder(MAVLTreeNode root, object[] list, int index)
        {
            if (root == null) return index - 1;
            int next = InternalInOrder(root.left, list, index);
            next++;
            list[next] = root.item;
            return InternalInOrder(root.right, list, next + 1);
        }

        #endregion
    }

    public static class MAVLTreeExtension
    {
        public static void Print(this MAVLTree tree)
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
                MAVLTreeNode node = (MAVLTreeNode)queue.Dequeue();
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
                MAVLTreeNode leftNode = node == null ? null : node.left;
                MAVLTreeNode rightNode = node == null ? null : node.right;
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

        public static void SortPrint(this MAVLTree tree)
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