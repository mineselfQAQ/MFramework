using MFramework;
using UnityEngine;

public class Test_MBinarySearchTree : MonoBehaviour
{
    private void Start()
    {
        MBinarySearchTree tree = new MBinarySearchTree();
        tree.Add(1);
        tree.Add(2);
        tree.Add(3);

        Log.Print("树1:");
        tree.Print();

        MBinarySearchTree tree2 = new MBinarySearchTree();
        tree2.Add(new MBinarySearchTreeNode(2));
        tree2.Add(new MBinarySearchTreeNode(1));
        tree2.Add(new MBinarySearchTreeNode(3));

        Log.Print("树2:");
        tree2.Print();

        MBinarySearchTree tree3 = new MBinarySearchTree();
        tree3.Add(3);
        tree3.Add(2);
        tree3.Add(1);

        Log.Print("树3:");
        tree3.Print();

        Log.Print(Log.ColorWord("---分隔符---", Color.red));

        //实现IEnumerable与Add()即可使用列表初始化
        tree = new MBinarySearchTree() { 2, 1, 5, 3, 7, 4, 6 };
        tree.Print();

        tree.Remove(1);//度0
        tree.Remove(3);//度1
        tree.Remove(tree.Root.Value);//root的度1
        tree.Remove(tree.Root);//度2
        tree.Print();

        Log.Print(Log.ColorWord("---分隔符---", Color.red));

        Log.Print("Count: " + tree.Count);
        Log.Print("是否包含6: " + tree.Contain(6));

        Log.Print(Log.ColorWord("---分隔符---", Color.red));

        object[] list = tree.Sort();
        Log.Print("list是否存在: " + list);
        tree.SortPrint();
    }
}
