using System;
using System.Collections;

namespace MFramework.DLC
{
    public class MCircularLinkedListNode
    {
        internal MCircularLinkedList list;

        internal MCircularLinkedListNode next;

        internal object item;

        public MCircularLinkedList List => list;

        public MCircularLinkedListNode Next
        {
            get
            {
                if (next != null)
                {
                    return next;
                }
                return null;
            }
        }

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

        //public的构造函数，可供外界提供Node使用
        public MCircularLinkedListNode(object value)
        {
            item = value;
        }
        //internal的构造函数，可供内部传入完整的Node
        internal MCircularLinkedListNode(MCircularLinkedList list, object value)
        {
            this.list = list;
            item = value;
        }

        //使节点无效(没有内容与其产生联系，可GC回收)
        internal void Invalidate()
        {
            list = null;
            next = null;
        }
    }

    public class MCircularLinkedList : IEnumerable
    {
        private MCircularLinkedListNode head;
        internal MCircularLinkedListNode tail;

        public MCircularLinkedListNode First => head;
        public MCircularLinkedListNode Last => tail;

        private int count;
        public int Count => count;

        public MCircularLinkedList() { }

        public MCircularLinkedListNode AddFirst(object value)
        {
            MCircularLinkedListNode newNode = new MCircularLinkedListNode(this, value);

            if (head == null)//还没有节点，放在头上
            {
                InternalInsertNodeEmpty(newNode);
            }
            else//插在头节点前面
            {
                InternalInsertNodeFirst(newNode);
            }

            return newNode;
        }
        public MCircularLinkedListNode AddFirst(MCircularLinkedListNode newNode)
        {
            ValidateNewNode(newNode);

            if (head == null)//还没有节点，放在头上
            {
                InternalInsertNodeEmpty(newNode);
            }
            else//插在头节点前面
            {
                InternalInsertNodeFirst(newNode);
            }

            newNode.list = this;
            return newNode;
        }
        public MCircularLinkedListNode AddLast(object value)
        {
            MCircularLinkedListNode newNode = new MCircularLinkedListNode(this, value);

            if (head == null)//还没有节点，放在头上
            {
                InternalInsertNodeEmpty(newNode);
            }
            else//插在尾节点后面
            {
                InternalInsertNodeLast(newNode);
            }

            return newNode;
        }
        public MCircularLinkedListNode AddLast(MCircularLinkedListNode newNode)
        {
            ValidateNewNode(newNode);

            if (head == null)//还没有节点，放在头上
            {
                InternalInsertNodeEmpty(newNode);
            }
            else//插在尾节点后面
            {
                InternalInsertNodeLast(newNode);
            }

            newNode.list = this;
            return newNode;
        }
        public MCircularLinkedListNode AddBefore(MCircularLinkedListNode node, object value)
        {
            ValidateNode(node);

            MCircularLinkedListNode newNode = new MCircularLinkedListNode(node.list, value);
            InternalInsertNodeBefore(node, newNode);

            return newNode;
        }
        public MCircularLinkedListNode AddBefore(MCircularLinkedListNode node, MCircularLinkedListNode newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);

            InternalInsertNodeBefore(node, newNode);

            newNode.list = this;
            return newNode;
        }
        public MCircularLinkedListNode AddAfter(MCircularLinkedListNode node, object value)
        {
            ValidateNode(node);

            MCircularLinkedListNode newNode = new MCircularLinkedListNode(node.list, value);
            InternalInsertNodeAfter(node, newNode);

            return newNode;
        }
        public MCircularLinkedListNode AddAfter(MCircularLinkedListNode node, MCircularLinkedListNode newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);

            InternalInsertNodeAfter(node, newNode);

            newNode.list = this;
            return newNode;
        }

        public void RemoveFirst()
        {
            if (head == null)
            {
                throw new Exception();
            }

            InternalRemoveNode(head);
        }
        public void RemoveLast()
        {
            if (head == null)
            {
                throw new Exception();
            }

            InternalRemoveNode(tail);
        }
        public bool Remove(object o)
        {
            MCircularLinkedListNode tempNode = Find(o);

            if (tempNode == null) return false;

            InternalRemoveNode(tempNode);
            return true;
        }

        public MCircularLinkedListNode Find(object o)
        {
            if (head == null) return null;

            MCircularLinkedListNode tempNode = head;
            //EqualityComparer<object> @default = EqualityComparer<object>.Default;

            if (o != null)
            {
                do
                {
                    //不能用==，必须用Equals()
                    if (tempNode.Value.Equals(o))
                    {
                        return tempNode;
                    }

                    tempNode = tempNode.next;
                }
                while (tempNode != null);
            }
            else
            {
                do
                {
                    if (tempNode.item == null)
                    {
                        return tempNode;
                    }

                    tempNode = tempNode.next;
                }
                while (tempNode != null);
            }

            return null;
        }
        public bool Contains(object o)
        {
            return Find(o) != null;
        }

        public void Clear()
        {
            //内存上的Clear
            MCircularLinkedListNode nextNode = head;
            int tempCount = count;
            while (tempCount != 0)
            {
                MCircularLinkedListNode tempNode = nextNode;
                nextNode = nextNode.next;
                tempNode.Invalidate();
                tempCount--;
            }

            head = null;
            tail = null;
            count = 0;
        }

        private void ValidateNode(MCircularLinkedListNode node)
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
        private void ValidateNewNode(MCircularLinkedListNode node)
        {
            //节点必须存在
            if (node == null)
            {
                throw new Exception();
            }
            //新节点的list应该为null
            if (node.list != null)
            {
                throw new Exception();
            }
        }

        //Tip:如果头节点还是尾节点有变动，对于循环链表来说，需要重新设置首尾相连
        private void InternalInsertNodeEmpty(MCircularLinkedListNode newNode)
        {
            head = newNode;
            tail = newNode;
            tail.next = head;//循环链表
            count++;
        }
        private void InternalInsertNodeFirst(MCircularLinkedListNode newNode)
        {
            newNode.next = head;
            head = newNode;
            tail.next = head;//循环链表
            count++;
        }
        private void InternalInsertNodeLast(MCircularLinkedListNode newNode)
        {
            tail.next = newNode;
            tail = newNode;
            tail.next = head;//循环链表
            count++;
        }
        private void InternalInsertNodeBefore(MCircularLinkedListNode node, MCircularLinkedListNode newNode)
        {
            MCircularLinkedListNode beforeNode = FindBeforeNode(node);

            //特殊情况---插入位置就是头节点
            if (beforeNode == null)
            {
                newNode.next = head;
                head = newNode;
                tail.next = head;//循环链表
                count++;
                return;
            }

            //一般情况---任意插入
            newNode.next = node;
            beforeNode.next = newNode;
            count++;
        }
        private void InternalInsertNodeAfter(MCircularLinkedListNode node, MCircularLinkedListNode newNode)
        {
            //特殊情况---插入位置就是尾节点
            if (node == tail)
            {
                tail.next = newNode;
                tail = newNode;
                tail.next = head;//循环链表
                count++;
                return;
            }

            //一般情况---任意插入
            newNode.next = node.next;
            node.next = newNode;
            count++;
        }
        private void InternalRemoveNode(MCircularLinkedListNode node)
        {
            if (head == tail)//只有一个元素情况
            {
                head = null;
                tail = null;
            }
            else
            {
                if (node == head)
                {
                    head = head.next;
                    tail.next = head;//循环链表
                }
                else if (node == tail)
                {
                    MCircularLinkedListNode beforeNode = FindBeforeNode(node);
                    tail = beforeNode;
                    tail.next = head;//循环链表
                }
                else
                {
                    MCircularLinkedListNode beforeNode = FindBeforeNode(node);
                    beforeNode.next = node.next;
                }
            }

            node.Invalidate();
            count--;
        }

        private MCircularLinkedListNode FindBeforeNode(MCircularLinkedListNode node)
        {
            //当查找节点已经是第一个节点时，没有前一个节点
            if (node == head) return null;

            //找到前一个节点
            MCircularLinkedListNode before = head;
            while (before.next != node && before != null)//before!=null可以避免意外情况
            {
                before = before.next;
            }
            return before;
        }


        public IEnumerator GetEnumerator()
        {
            MCircularLinkedListNode nextNode = head;

            int tempCount = count;

            while (tempCount != 0)
            {
                yield return nextNode.Value;
                nextNode = nextNode.next;
                tempCount--;
            }
        }
    }

    public static class MCircularLinkedListExtension
    {
        public static void Print(this MCircularLinkedList list)
        {
            MLog.Print("输出: ");

            if (list.Count == 0)
            {
                MLog.Print("无元素");
                return;
            }

            string outputStr = "";
            foreach (var item in list)
            {
                outputStr += $"{item} ";
            }
            MLog.Print(outputStr);
        }
    }
}