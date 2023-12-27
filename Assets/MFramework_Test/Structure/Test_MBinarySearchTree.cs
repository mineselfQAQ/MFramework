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

        tree.Print();
    }
}
