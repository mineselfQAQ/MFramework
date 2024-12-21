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
        MLog.Print("是否包含5: " + tree.Contains(5));
        MLog.Print("2的值: " + leftNode.Value);
        MLog.Print("2的父节点: " + leftNode.Parent);
        MLog.Print("2的左节点: " + leftNode.Left);
        MLog.Print("2的右节点: " + leftNode.Right);

        MLog.Print(MLog.Color("---分隔符---", Color.red));

        tree.Print();
    }
}
