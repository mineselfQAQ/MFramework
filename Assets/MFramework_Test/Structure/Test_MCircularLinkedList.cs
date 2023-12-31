using MFramework;
using UnityEngine;

public class Test_MCircularLinkedList : MonoBehaviour
{
    private void Start()
    {
        MCircularLinkedListNode tempNode;
        MCircularLinkedList list = new MCircularLinkedList();
        tempNode = list.AddFirst(2);
        list.AddLast(4);
        list.AddBefore(tempNode, 1);
        list.AddAfter(tempNode, new MCircularLinkedListNode(3));

        list.Print();

        Log.Print(Log.ColorWord("---分隔符---", Color.red));

        Log.Print("链表元素数量: " + list.Count);
        Log.Print("头节点值: " + list.First.Value);
        Log.Print("尾节点值: " + list.Last.Value);
        Log.Print("第二个节点值: " + list.First.Next.Value);
        Log.Print("是否存在3: " + list.Find(3));
        Log.Print("是否存在3: " + list.Contains(3));
        Log.Print("第二个节点所属链表: " + tempNode.list);

        Log.Print(Log.ColorWord("---分隔符---", Color.red));

        list.RemoveFirst();
        list.RemoveLast();
        list.Remove(2);

        list.Print();

        list.Clear();

        list.Print();
    }
}
