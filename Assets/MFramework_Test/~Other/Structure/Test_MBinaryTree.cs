using MFramework;
using MFramework.DLC;
using UnityEngine;

public class Test_MBinaryTree : MonoBehaviour
{
    private void Start()
    {
        MBinaryTreeNode tempNode;
        MBinaryTree tree = new MBinaryTree(1);

        MBinaryTreeNode leftNode = tree.AddLeft(tree.Root, 2);
        MBinaryTreeNode rightNode = tree.AddRight(tree.Root, 3);

        tree.AddLeft(leftNode, 4);
        tempNode = tree.AddRight(leftNode, 5);
        tree.AddLeft(rightNode, 6);
        tree.AddRight(rightNode, 7);
        tree.Remove(tempNode);

        MLog.Print("Count: " + tree.Count);
        MLog.Print("�Ƿ����5: " + tree.Contains(5));
        MLog.Print("2��ֵ: " + leftNode.Value);
        MLog.Print("2�ĸ��ڵ�: " + leftNode.Parent);
        MLog.Print("2����ڵ�: " + leftNode.Left);
        MLog.Print("2���ҽڵ�: " + leftNode.Right);

        MLog.Print(MLog.Color("---�ָ���---", Color.red));

        tree.Print();
    }
}
