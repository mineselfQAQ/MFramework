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

        MLog.Print(MLog.Color("---�ָ���---", Color.red));

        MLog.Print("����Ԫ������: " + list.Count);
        MLog.Print("ͷ�ڵ�ֵ: " + list.First.Value);
        MLog.Print("β�ڵ�ֵ: " + list.Last.Value);
        MLog.Print("�ڶ����ڵ�ֵ: " + list.First.Next.Value);
        MLog.Print("�Ƿ����3: " + list.Find(3));
        MLog.Print("�Ƿ����3: " + list.Contains(3));
        MLog.Print("�ڶ����ڵ���������: " + tempNode.List);

        MLog.Print(MLog.Color("---�ָ���---", Color.red));

        list.RemoveFirst();
        list.RemoveLast();
        list.Remove(2);

        list.Print();

        list.Clear();

        list.Print();
    }
}
