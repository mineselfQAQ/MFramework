using MFramework;
using MFramework.DLC;
using System;
using System.Collections;

public enum RBColor
{
    Black,
    Red
}

public class MRBTreeNode
{
    #region Internal Field
    internal MRBTree list;

    internal object item;

    internal MRBTreeNode parent;

    internal MRBTreeNode left;

    internal MRBTreeNode right;

    internal RBColor color;
    #endregion

    #region Public Field
    public MRBTree List => list;

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

    public MRBTreeNode Parent => parent;

    public MRBTreeNode Left => left;

    public MRBTreeNode Right => right;

    /// <summary>
    /// 颜色值---红/黑
    /// </summary>
    public RBColor Color => color;
    #endregion

    #region Constructor
    public MRBTreeNode(object value)
    {
        item = value;
    }

    internal MRBTreeNode(object value, MRBTree list)
    {
        this.list = list;
        item = value;
    }
    #endregion

    #region Function
    //使节点无效(没有内容与其产生联系，可GC回收)
    internal void Invalidate()
    {
        //父节点断开
        if (parent.left == this) parent.left = null;
        else parent.right = null;

        //自身断开
        list = null;//核心---list清空将通不过ValidateNode()，从而无法进行任何操作
        parent = null;
        left = null;
        right = null;
    }
    #endregion
}

public class MRBTree : IEnumerable
{
    #region Internal Field
    private MRBTreeNode root;

    private int count;
    #endregion


    #region Public Field
    public MRBTreeNode Root => root;

    public int Count => count;
    #endregion

    #region Constructor
    public MRBTree() { }
    #endregion

    #region Function
    private MRBTreeNode GetParent(MRBTreeNode node) => node?.parent;
    private MRBTreeNode GetGrandParent(MRBTreeNode node) => GetParent(GetParent(node));
    /// <summary>
    /// 获取父节点的兄弟节点(即叔叔节点)
    /// </summary>
    private MRBTreeNode GetUncle(MRBTreeNode node)
    {
        //先找祖父节点
        var gpNode = GetGrandParent(node);
        if (gpNode == null) return null;

        //祖父节点与当前节点连接的另一面就是叔叔节点
        if (gpNode.left == node.parent)
        {
            return gpNode.right;
        }
        else
        {
            return gpNode.left;
        }
    }
    private MRBTreeNode GetBrother(MRBTreeNode node)
    {
        var pNode = GetParent(node);
        if (pNode == null) return null;

        if (pNode.right == node)//右向
        {
            return pNode?.left;//兄弟节点在左向
        }
        else//左向
        {
            return pNode?.right;//兄弟节点在右向
        }
    }

    private bool isRed(MRBTreeNode node) => node.color == RBColor.Red;
    private bool isBlack(MRBTreeNode node)
    {
        if (node == null) return true;//叶子节点(真叶子节点的子节点)必定为黑色

        return node.color == RBColor.Black;
    }
    private void SetRed(MRBTreeNode node)
    {
        if (node == null) return;
        node.color = RBColor.Red;
    }
    private void SetBlack(MRBTreeNode node)
    {
        if (node == null) return;
        node.color = RBColor.Black;
    }

    public void Add(object o)
    {
        var node = InternalAdd(o);
        AddFix(node);
        UpdateRoot();
        count++;
    }
    public void Remove(object o)
    {
        if (root == null) throw new Exception();//root为空那么不可能有内容需要remove

        var node = InternalRemove(o);//所需删除的node
        RemoveFix(node);//整理结构
        UpdateRoot();
        node.Invalidate();//真正的删除
        count--;
    }

    /// <returns>新添加Node</returns>
    private MRBTreeNode InternalAdd(object o)
    {
        MRBTreeNode newNode = new MRBTreeNode(o, this);

        if (root == null)
        {
            root = newNode;
            SetBlack(newNode);
            return newNode;
        }

        IComparable num = newNode.item as IComparable;
        if (num == null) throw new Exception();//对于没有IComparable的数据不能比较

        MRBTreeNode cur = root;

        while (true)
        {
            if (num.CompareTo(cur.item) > 0)//num > cur.item
            {
                if (cur.right == null)
                {
                    newNode.parent = cur;
                    cur.right = newNode;
                    SetRed(newNode);
                    return newNode;
                }

                cur = cur.right;//传入值更大，向右走
            }
            else if (num.CompareTo(cur.item) < 0)//num < cur.item
            {
                if (cur.left == null)
                {
                    newNode.parent = cur;
                    cur.left = newNode;
                    SetRed(newNode);
                    return newNode;
                }

                cur = cur.left;//传入值更小，向左走
            }
            else
            {
                throw new Exception();//一颗二叉搜索树中不可能有相同数字
            }
        }
    }
    private void AddFix(MRBTreeNode node)
    {
        //1.节点为根节点，无需修复
        if (root == node) return;

        var pNode = GetParent(node);
        //2.父节点为黑色，无需修复
        if (pNode != null && isBlack(pNode)) return;

        //3.父节点为红色
        var gpNode = GetParent(pNode);//必定存在(父节点为红色，那么必定附着与黑色节点)
        var uNode = GetUncle(node);
        //3.1.叔叔节点为黑色(即没有另一侧红色节点)
        if (uNode == null || (uNode != null && isBlack(uNode)))
        {
            //3.1.1.LL情况
            if (gpNode?.left == pNode && pNode?.left == node)
            {
                //祖父节点右旋
                var newGPNode = RightRotate(gpNode);
                //染色
                SetBlack(newGPNode);
                SetRed(newGPNode.right);
            }
            //3.1.2.RR情况
            else if (gpNode?.right == pNode && pNode?.right == node)
            {
                //祖父节点左旋
                var newGPNode = LeftRotate(gpNode);
                //染色
                SetBlack(newGPNode);
                SetRed(newGPNode.left);
            }
            //3.1.3.LR情况
            else if (gpNode?.left == pNode && pNode?.right == node)
            {
                //父节点左旋后，祖父节点右旋
                LeftRotate(pNode);
                var newGPNode = RightRotate(gpNode);
                //染色
                SetBlack(newGPNode);
                SetRed(newGPNode.right);
            }
            //3.1.4.RL情况
            else if (gpNode?.right == pNode && pNode?.left == node)
            {
                //父节点右旋后，祖父节点左旋
                RightRotate(pNode);
                var newGPNode = LeftRotate(gpNode);
                //染色
                SetBlack(newGPNode);
                SetRed(newGPNode.left);
            }
        }
        //3.2.叔叔节点为红色
        else if(uNode != null && isRed(uNode))
        {
            SetRed(gpNode);
            SetBlack(gpNode.left);
            SetBlack(gpNode.right);

            AddFix(gpNode);
        }
    }
    private void UpdateRoot()
    {
        while (root.parent != null)
        {
            root = root.parent;
        }
        SetBlack(root);
    }

    private MRBTreeNode InternalRemove(object o)
    {
        var node = Search(o);

        if (node == null)
        {
            throw new Exception();
        }

        //度为0(为叶子节点)
        if (node.left == null && node.right == null)
        {
            return node;
        }
        else//不为叶子节点，需要先找到前驱或后继节点(这里选择前驱)
        {
            var tempNode = node;

            if (node.left == null)//只有右子节点
            {
                //获取前驱节点
                node = node.right;
                while (node.left != null)
                {
                    node = node.left;
                }
            }
            else//一般情况
            {
                //获取后继节点
                node = node.left;
                while (node.right != null)
                {
                    node = node.right;
                }
            }

            //前驱节点移至删除节点(即变换为删除前驱节点)
            tempNode.item = node.item;

            return node;
        }
    }
    private void RemoveFix(MRBTreeNode node, bool isLeaf = true)
    {
        //1.删除节点为根节点
        if (root == node)
        {
            //又是根节点，又是叶子节点，这意味着只有1个节点，直接清空即可
            root = null;
            return;
        }

        var pNode = GetParent(node);
        var bNode = GetBrother(node);
        bool black = isBlack(node);//删除节点颜色
        var cNode = node.left ?? node.right;//获取唯一红色节点

        //2.1.删除节点为红色节点
        //只需删除即可，无需额外处理
        //2.2.删除节点为黑色节点
        if (black)
        {
            //2.2.1.拥有0个红色节点
            if (cNode == null || !isLeaf)
            {
                //Tip：此时必须有兄弟节点，否则不满足2-3-4树

                //2.2.1.1.兄弟节点为黑色
                if (isBlack(bNode))
                {
                    //2.2.1.1.1.无红色节点
                    if (isBlack(bNode.left) && isBlack(bNode.right))
                    {
                        //2.2.1.1.1.1.父节点为红色
                        if (isRed(pNode))
                        {
                            //无法向兄弟节点"借"节点，只能下溢
                            //即：父节点下溢并变黑，兄弟节点变红
                            SetBlack(pNode);
                            SetRed(bNode);
                        }
                        //2.2.1.1.1.2.父节点为黑色
                        else
                        {
                            //同2.2.1.1.1.1
                            SetBlack(pNode);
                            SetRed(bNode);
                            //但是：此时由于删除节点并将兄弟节点至红，"所有路径黑色节点数量一致"不满足，需要递归处理
                            RemoveFix(pNode, false);
                        }
                    }
                    //2.2.1.1.2.有1个/2个红色节点
                    else
                    {
                        //XL
                        if (bNode.left != null && isRed(bNode.left))
                        {
                            //LL
                            if (pNode.left == bNode)
                            {
                                var newPNode = RightRotate(pNode);
                                SetBlack(newPNode.right);
                            }
                            //RL
                            else if(pNode.right == bNode)
                            {
                                RightRotate(bNode);
                                var newPNode = LeftRotate(pNode);
                                SetBlack(newPNode.left);
                            }
                        }
                        //XR
                        else if (bNode.right != null && isRed(bNode.right))
                        {
                            //RR
                            if (pNode.right == bNode)
                            {
                                var newPNode = LeftRotate(pNode);
                                SetBlack(newPNode.left);
                            }
                            //LR
                            else if (pNode.left == bNode)
                            {
                                LeftRotate(bNode);
                                var newPNode = RightRotate(pNode);
                                SetBlack(newPNode.right);
                            }
                        }
                    }
                }
                //2.2.1.2.兄弟节点为红色
                else
                {
                    if (pNode.left == bNode)//兄弟在左侧
                    {
                        var newPNode = RightRotate(pNode);
                        SetBlack(newPNode);
                        SetRed(newPNode.right);
                        RemoveFix(node, isLeaf);
                    }
                    else//兄弟在右侧
                    {
                        var newPNode = LeftRotate(pNode);
                        SetBlack(newPNode);
                        SetRed(newPNode.left);
                        RemoveFix(node, isLeaf);
                    }
                }
            }
            else
            {
                throw new Exception();
            }
            //2.2.2.拥有1个红色节点
            //不可能发生，因为：
            //拥有1个红色节点意味着是节点是红色节点的父节点，与叶子节点相斥
            //else
            //{
            //    bool isRight = pNode?.right == node ? true : false;//父节点指向
            //    if (isRight)
            //    {
            //        //保持父节点指向
            //        pNode.right = cNode;
            //        cNode.parent = pNode;
            //        SetBlack(cNode);
            //    }
            //    else
            //    {
            //        //保持父节点指向
            //        pNode.left = cNode;
            //        cNode.parent = pNode;
            //        SetBlack(cNode);
            //    }
            //}
            //2.2.3.拥有2个红色节点
            //不可能发生，因为：
            //拥有2个红色节点意味着是节点是红色节点的父节点，与叶子节点相斥
        }
    }

    public bool Contains(object o)
    {
        return Search(o) != null;
    }
    private MRBTreeNode Search(object o)
    {
        IComparable num = o as IComparable;
        if (num == null) throw new Exception();//对于没有IComparable的数据不能比较

        MRBTreeNode cur = root;

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

    private MRBTreeNode RightRotate(MRBTreeNode node)
    {
        MRBTreeNode parentNode = node.parent;
        MRBTreeNode childNode = node.left;

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
            MRBTreeNode grandChildNode = childNode.right;
            childNode.parent = node.parent;
            childNode.right = node;
            node.left = grandChildNode;
            node.parent = childNode;
            grandChildNode.parent = node;
        }

        //设置父节点的子节点
        if (parentNode == null) return childNode;
        bool isRight = parentNode.right == node ? true : false;
        if (isRight)//向右连接
        {
            parentNode.right = childNode;
        }
        else//向左连接
        {
            parentNode.left = childNode;
        }

        return childNode;//为更新后的"根节点"
    }
    private MRBTreeNode LeftRotate(MRBTreeNode node)
    {
        MRBTreeNode parentNode = node.parent;
        MRBTreeNode childNode = node.right;

        //理论上不会发生，但是如果左子节点都没找到，那么就不可能右旋
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
            MRBTreeNode grandChildNode = childNode.left;
            childNode.parent = node.parent;
            childNode.left = node;
            node.right = grandChildNode;
            node.parent = childNode;
            grandChildNode.parent = node;
        }

        //设置父节点的子节点
        if (parentNode == null) return childNode;
        bool isRight = parentNode.right == node ? true : false;
        if (isRight)//向右连接
        {
            parentNode.right = childNode;
        }
        else//向左连接
        {
            parentNode.left = childNode;
        }

        return childNode;//为更新后的"根节点"
    }

    private void ValidateNode(MRBTreeNode node)
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
    private int InternalInOrder(MRBTreeNode root, object[] list, int index)
    {
        if (root == null) return index - 1;
        int next = InternalInOrder(root.left, list, index);
        next++;
        list[next] = root.item;
        return InternalInOrder(root.right, list, next + 1);
    }
    #endregion
}

public static class MRBTreeExtension
{
    public static void Print(this MRBTree tree)
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
            MRBTreeNode node = (MRBTreeNode)queue.Dequeue();
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
            MRBTreeNode leftNode = node == null ? null : node.left;
            MRBTreeNode rightNode = node == null ? null : node.right;
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

    public static void SortPrint(this MRBTree tree)
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
