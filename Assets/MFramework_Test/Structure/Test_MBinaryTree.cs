using MFramework;
using UnityEngine;

public class Test_MBinaryTree : MonoBehaviour
{
    private void Start()
    {
        MBinaryTree tree = new MBinaryTree(1);

        MBinaryTreeNode leftNode = tree.AddLeft(tree.Root, 2);
        MBinaryTreeNode rightNode = tree.AddRight(tree.Root, 3);

        tree.AddLeft(leftNode, 4);
        //tree.AddRight(leftNode, 5);
        tree.AddLeft(rightNode, 6);
        tree.AddRight(rightNode, 7);

        // µ—È---≤„–Ú±È¿˙
        string outputStr = "";
        foreach (var item in tree.InOrder())
        {
            outputStr += $"{item} ";
        }
        Log.Print(outputStr);
    }
}
