using MFramework;
using MFramework.DLC;
using UnityEngine;

public class Test_MBinarySearchTree : MonoBehaviour
{
    private void Start()
    {
        MBinarySearchTree tree = new MBinarySearchTree();
        tree.Add(1);
        tree.Add(2);
        tree.Add(3);

        MLog.Print("��1:");
        tree.Print();

        MBinarySearchTree tree2 = new MBinarySearchTree();
        tree2.Add(new MBinarySearchTreeNode(2));
        tree2.Add(new MBinarySearchTreeNode(1));
        tree2.Add(new MBinarySearchTreeNode(3));

        MLog.Print("��2:");
        tree2.Print();

        MBinarySearchTree tree3 = new MBinarySearchTree();
        tree3.Add(3);
        tree3.Add(2);
        tree3.Add(1);

        MLog.Print("��3:");
        tree3.Print();

        MLog.Print(MLog.Color("---�ָ���---", Color.red));

        //ʵ��IEnumerable��Add()����ʹ���б��ʼ��
        tree = new MBinarySearchTree() { 2, 1, 5, 3, 7, 4, 6 };
        tree.Print();

        tree.Remove(1);//��0
        tree.Remove(3);//��1
        tree.Remove(tree.Root.Value);//root�Ķ�1
        tree.Remove(tree.Root);//��2
        tree.Print();

        MLog.Print(MLog.Color("---�ָ���---", Color.red));

        MLog.Print("Count: " + tree.Count);
        MLog.Print("�Ƿ����6: " + tree.Contains(6));

        MLog.Print(MLog.Color("---�ָ���---", Color.red));

        object[] list = tree.Sort();
        MLog.Print("list�Ƿ����: " + list);
        tree.SortPrint();
    }
}
