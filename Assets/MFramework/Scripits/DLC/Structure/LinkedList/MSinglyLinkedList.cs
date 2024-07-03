using System;
using System.Collections;

namespace MFramework.DLC
{
    public class MSinglyLinkedListNode
    {
        internal MSinglyLinkedList list;

        internal MSinglyLinkedListNode next;

        internal object item;

        public MSinglyLinkedList List => list;

        public MSinglyLinkedListNode Next
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
        public MSinglyLinkedListNode(object value)
        {
            item = value;
        }
        //internal的构造函数，可供内部传入完整的Node
        internal MSinglyLinkedListNode(MSinglyLinkedList list, object value)
        {
            this.list = list;
            item = value;
        }

        //使节点无效(没有内容与其产生联系，可GC回收)
        internal void Invalidate()
        {
            //不在这里切断前Node与该Node的联系，而是放在移除操作InternalRemoveNode()中
            //对于一般情况，移除会将前后Node联系起来，只需要对自己的引用置为null即可
            //只有移除尾节点时，才会发生引用处理不全
            //MSinglyLinkedListNode beforeNode = list.FindBeforeNode(this);
            //if (beforeNode != null)
            //{
            //    beforeNode.next = null;
            //}

            list = null;
            next = null;
        }
    }

    public class MSinglyLinkedList : IEnumerable
    {
        private MSinglyLinkedListNode head;
        private MSinglyLinkedListNode tail;

        public MSinglyLinkedListNode First => head;
        public MSinglyLinkedListNode Last => tail;

        private int count;
        public int Count => count;

        public MSinglyLinkedList() { }

        public MSinglyLinkedListNode AddFirst(object value)
        {
            MSinglyLinkedListNode newNode = new MSinglyLinkedListNode(this, value);

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
        public MSinglyLinkedListNode AddFirst(MSinglyLinkedListNode newNode)
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
        public MSinglyLinkedListNode AddLast(object value)
        {
            MSinglyLinkedListNode newNode = new MSinglyLinkedListNode(this, value);

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
        public MSinglyLinkedListNode AddLast(MSinglyLinkedListNode newNode)
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
        public MSinglyLinkedListNode AddBefore(MSinglyLinkedListNode node, object value)
        {
            ValidateNode(node);

            MSinglyLinkedListNode newNode = new MSinglyLinkedListNode(node.list, value);
            InternalInsertNodeBefore(node, newNode);

            return newNode;
        }
        public MSinglyLinkedListNode AddBefore(MSinglyLinkedListNode node, MSinglyLinkedListNode newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);

            InternalInsertNodeBefore(node, newNode);

            newNode.list = this;
            return newNode;
        }
        public MSinglyLinkedListNode AddAfter(MSinglyLinkedListNode node, object value)
        {
            ValidateNode(node);

            MSinglyLinkedListNode newNode = new MSinglyLinkedListNode(node.list, value);
            InternalInsertNodeAfter(node, newNode);

            return newNode;
        }
        public MSinglyLinkedListNode AddAfter(MSinglyLinkedListNode node, MSinglyLinkedListNode newNode)
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
            MSinglyLinkedListNode tempNode = Find(o);

            if (tempNode == null) return false;

            InternalRemoveNode(tempNode);
            return true;
        }
        public bool Remove(MSinglyLinkedListNode node)
        {
            ValidateNode(node);
            InternalRemoveNode(node);

            return true;
        }

        public MSinglyLinkedListNode Find(object o)
        {
            if (head == null) return null;

            MSinglyLinkedListNode tempNode = head;
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
            MSinglyLinkedListNode nextNode = head;
            while (nextNode != null)
            {
                MSinglyLinkedListNode tempNode = nextNode;
                nextNode = nextNode.next;
                tempNode.Invalidate();
            }

            head = null;
            tail = null;
            count = 0;
        }

        private void ValidateNode(MSinglyLinkedListNode node)
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
        private void ValidateNewNode(MSinglyLinkedListNode node)
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

        private void InternalInsertNodeEmpty(MSinglyLinkedListNode newNode)
        {
            newNode.next = null;
            head = newNode;
            tail = newNode;
            count++;
        }
        private void InternalInsertNodeFirst(MSinglyLinkedListNode newNode)
        {
            newNode.next = head;
            head = newNode;
            count++;
        }
        private void InternalInsertNodeLast(MSinglyLinkedListNode newNode)
        {
            tail.next = newNode;
            tail = newNode;
            count++;
        }
        private void InternalInsertNodeBefore(MSinglyLinkedListNode node, MSinglyLinkedListNode newNode)
        {
            MSinglyLinkedListNode beforeNode = FindBeforeNode(node);

            //特殊情况---插入位置就是头节点
            if (beforeNode == null)
            {
                newNode.next = head;
                head = newNode;
                count++;
                return;
            }

            //一般情况---任意插入
            newNode.next = node;
            beforeNode.next = newNode;
            count++;
        }
        private void InternalInsertNodeAfter(MSinglyLinkedListNode node, MSinglyLinkedListNode newNode)
        {
            //特殊情况---插入位置就是尾节点
            if (node == tail)
            {
                tail.next = newNode;
                tail = newNode;
                count++;
                return;
            }

            //一般情况---任意插入
            newNode.next = node.next;
            node.next = newNode;
            count++;
        }
        private void InternalRemoveNode(MSinglyLinkedListNode node)
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
                }
                else if (node == tail)
                {
                    MSinglyLinkedListNode beforeNode = FindBeforeNode(node);
                    beforeNode.next = null;//需要切断尾节点与前节点的联系
                    tail = beforeNode;
                }
                else
                {
                    MSinglyLinkedListNode beforeNode = FindBeforeNode(node);
                    beforeNode.next = node.next;
                }
            }

            node.Invalidate();
            count--;
        }

        internal MSinglyLinkedListNode FindBeforeNode(MSinglyLinkedListNode node)
        {
            //当查找节点已经是第一个节点时，没有前一个节点
            if (node == head) return null;

            //找到前一个节点
            MSinglyLinkedListNode before = head;
            while (before.next != node && before != null)//before!=null可以避免意外情况
            {
                before = before.next;
            }
            return before;
        }

        public IEnumerator GetEnumerator()
        {
            MSinglyLinkedListNode nextNode = head;
            while (nextNode != null)
            {
                yield return nextNode.Value;
                nextNode = nextNode.next;
            }
        }
    }

    public static class MSinglyLinkedListExtension
    {
        public static void Print(this MSinglyLinkedList list)
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