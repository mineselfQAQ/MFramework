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

        Log.Print("Ê÷1:");
        tree.Print();

        Log.Print("Ê÷2:");
        MBinarySearchTree tree2 = new MBinarySearchTree();
        tree2.Add(2);
        tree2.Add(1);
        tree2.Add(3);
        tree2.Print();

        Log.Print("Ê÷3:");
        MBinarySearchTree tree3 = new MBinarySearchTree();
        tree3.Add(3);
        tree3.Add(2);
        tree3.Add(1);
        tree3.Print();
    }
}
