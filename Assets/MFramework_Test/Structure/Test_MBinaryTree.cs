using MFramework;
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

        Log.Print("Count: " + tree.Count);
        Log.Print("是否包含5: " + tree.Contains(5));
        Log.Print("2的值: " + leftNode.Value);
        Log.Print("2的父节点: " + leftNode.Parent);
        Log.Print("2的左节点: " + leftNode.Left);
        Log.Print("2的右节点: " + leftNode.Right);

        Log.Print(Log.ColorWord("---分隔符---", Color.red));

        tree.Print();
    }
}
