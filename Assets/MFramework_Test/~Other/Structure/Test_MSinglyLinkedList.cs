using MFramework;
using MFramework.DLC;
using UnityEngine;

public class Test_MSinglyLinkedList : MonoBehaviour
{
    private void Start()
    {
        MSinglyLinkedListNode tempNode;
        MSinglyLinkedList list = new MSinglyLinkedList();
        tempNode = list.AddFirst(2);
        list.AddLast(4);
        list.AddBefore(tempNode, 1);
        list.AddAfter(tempNode, new MSinglyLinkedListNode(3));


        list.Print();

        MLog.Print(MLog.Color("---分隔符---", Color.red));

        MLog.Print("链表元素数量: " + list.Count);
        MLog.Print("头节点值: " + list.First.Value);
        MLog.Print("尾节点值: " + list.Last.Value);
        MLog.Print("第二个节点值: " + list.First.Next.Value);
        MLog.Print("是否存在3: " + list.Find(3));
        MLog.Print("是否存在3: " + list.Contains(3));
        MLog.Print("第二个节点所属链表: " + tempNode.List);

        MLog.Print(MLog.Color("---分隔符---", Color.red));

        list.RemoveFirst();
        list.RemoveLast();
        list.Remove(2);

        list.Print();

        list.Clear();

        list.Print();
    }
}
