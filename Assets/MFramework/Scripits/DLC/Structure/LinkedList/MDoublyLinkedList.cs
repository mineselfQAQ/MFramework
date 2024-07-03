using System;
using System.Collections;

namespace MFramework.DLC
{
    public class MDoublyLinkedListNode
    {
        internal MDoublyLinkedList list;

        internal MDoublyLinkedListNode next;

        internal MDoublyLinkedListNode prev;

        internal object item;

        public MDoublyLinkedList List => list;

        public MDoublyLinkedListNode Next
        {
            get
            {
                //限制：必须有下一Node且该Node不是尾节点
                //禁止了循环操作，这对整体是有利的：
                //对尾节点进行Next操作，
                //如果可以取出，那么会得到头节点，这里的意义是不大的，可以直接获取First
                //如果不可以取出，那么会得到null，我们可以用该条件进行判断该节点是否为尾节点
                if (next != null && next != list.head)
                {
                    return next;
                }
                return null;
            }
        }

        public MDoublyLinkedListNode Previous
        {
            get
            {
                //同Next
                if (prev != null && this != list.head)
                {
                    return prev;
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
        public MDoublyLinkedListNode(object value)
        {
            item = value;
        }
        //internal的构造函数，可供内部传入完整的Node
        internal MDoublyLinkedListNode(MDoublyLinkedList list, object value)
        {
            this.list = list;
            item = value;
        }

        //使节点无效(没有内容与其产生联系，可GC回收)
        internal void Invalidate()
        {
            list = null;
            next = null;
            prev = null;
        }
    }

    public class MDoublyLinkedList : IEnumerable
    {
        internal MDoublyLinkedListNode head;

        public MDoublyLinkedListNode First => head;
        public MDoublyLinkedListNode Last
        {
            get
            {
                if (head != null)//头节点都没有就不存在头节点的前一个节点
                {
                    return head.prev;
                }
                return null;
            }
        }

        private int count;
        public int Count => count;

        public MDoublyLinkedList() { }

        public MDoublyLinkedListNode AddFirst(object value)
        {
            MDoublyLinkedListNode newNode = new MDoublyLinkedListNode(this, value);

            if (head == null)//还没有节点，放在头上
            {
                InternalInsertNodeEmpty(newNode);
            }
            else//插在头节点前面
            {
                InternalInsertNodeBefore(head, newNode);
                head = newNode;
            }

            return newNode;
        }
        public MDoublyLinkedListNode AddFirst(MDoublyLinkedListNode newNode)
        {
            ValidateNewNode(newNode);

            if (head == null)//还没有节点，放在头上
            {
                InternalInsertNodeEmpty(newNode);
            }
            else//插在头节点前面
            {
                InternalInsertNodeBefore(head, newNode);
                head = newNode;
            }

            newNode.list = this;
            return newNode;
        }
        public MDoublyLinkedListNode AddLast(object value)
        {
            MDoublyLinkedListNode newNode = new MDoublyLinkedListNode(this, value);

            if (head == null)//还没有节点，放在头上
            {
                InternalInsertNodeEmpty(newNode);
            }
            else//插在尾节点后面
            {
                InternalInsertNodeBefore(head, newNode);//尾节点后面其实就是头节点前面
            }

            return newNode;
        }
        public MDoublyLinkedListNode AddLast(MDoublyLinkedListNode newNode)
        {
            ValidateNewNode(newNode);

            if (head == null)//还没有节点，放在头上
            {
                InternalInsertNodeEmpty(newNode);
            }
            else//插在尾节点后面
            {
                InternalInsertNodeBefore(head, newNode);//尾节点后面其实就是头节点前面
            }

            newNode.list = this;
            return newNode;
        }
        public MDoublyLinkedListNode AddBefore(MDoublyLinkedListNode node, object value)
        {
            ValidateNode(node);

            MDoublyLinkedListNode newNode = new MDoublyLinkedListNode(node.list, value);
            InternalInsertNodeBefore(node, newNode);

            //特殊情况---插到头节点前，需要设置新的头节点
            if (node == head)
            {
                head = newNode;
            }

            return newNode;
        }
        public MDoublyLinkedListNode AddBefore(MDoublyLinkedListNode node, MDoublyLinkedListNode newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);

            InternalInsertNodeBefore(node, newNode);
            newNode.list = this;

            //特殊情况---插到头节点前，需要设置新的头节点
            if (node == head)
            {
                head = newNode;
            }

            return newNode;
        }
        public MDoublyLinkedListNode AddAfter(MDoublyLinkedListNode node, object value)
        {
            ValidateNode(node);

            MDoublyLinkedListNode newNode = new MDoublyLinkedListNode(node.list, value);
            InternalInsertNodeBefore(node.next, newNode);//插到后面，其实就是插到后节点的前面

            return newNode;
        }
        public MDoublyLinkedListNode AddAfter(MDoublyLinkedListNode node, MDoublyLinkedListNode newNode)
        {
            ValidateNode(node);
            ValidateNewNode(newNode);

            InternalInsertNodeBefore(node.next, newNode);//插到后面，其实就是插到后节点的前面
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

            InternalRemoveNode(head.prev);
        }
        public bool Remove(object o)
        {
            MDoublyLinkedListNode tempNode = Find(o);

            if (tempNode == null) return false;

            InternalRemoveNode(tempNode);
            return true;
        }
        public bool Remove(MDoublyLinkedListNode node)
        {
            ValidateNode(node);
            InternalRemoveNode(node);

            return true;
        }

        public MDoublyLinkedListNode Find(object o)
        {
            if (head == null) return null;

            MDoublyLinkedListNode tempNode = head;
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
                while (tempNode != head);
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
                while (tempNode != head);
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
            MDoublyLinkedListNode nextNode = head;
            while (nextNode != null)
            {
                MDoublyLinkedListNode tempNode = nextNode;
                nextNode = nextNode.Next;//必须为属性Next，以防止循环
                tempNode.Invalidate();
            }

            head = null;
            count = 0;
        }

        private void ValidateNode(MDoublyLinkedListNode node)
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
        private void ValidateNewNode(MDoublyLinkedListNode node)
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

        private void InternalInsertNodeEmpty(MDoublyLinkedListNode newNode)
        {
            newNode.next = newNode;//自循环
            newNode.prev = newNode;//自循环
            head = newNode;
            count++;
        }
        private void InternalInsertNodeBefore(MDoublyLinkedListNode node, MDoublyLinkedListNode newNode)
        {
            newNode.next = node;
            newNode.prev = node.prev;
            node.prev.next = newNode;
            node.prev = newNode;

            count++;
        }
        private void InternalRemoveNode(MDoublyLinkedListNode node)
        {
            if (node.next == node)//只有一个节点
            {
                head = null;
            }
            else
            {
                node.next.prev = node.prev;
                node.prev.next = node.next;

                //特殊情况---删除节点为头节点，那么新的头节点就是下一个节点
                if (head == node)
                {
                    head = node.next;
                }
            }

            node.Invalidate();
            count--;
        }

        public IEnumerator GetEnumerator()
        {
            MDoublyLinkedListNode nextNode = head;
            while (nextNode != null)
            {
                yield return nextNode.Value;
                nextNode = nextNode.Next;
            }
        }
    }

    public static class MDoublyLinkedListExtension
    {
        public static void Print(this MDoublyLinkedList list)
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