using MFramework;
using UnityEngine;

public class Test_MAVLTree : MonoBehaviour
{
    private void Start()
    {
        MAVLTree tree = new MAVLTree();
        tree.Add(2);
        tree.Add(1);
        tree.Add(8);
        tree.Add(4);
        tree.Add(3);
        tree.Add(7);
        tree.Add(9);
        tree.Add(6);
        tree.Add(5);

        tree.Print();
    }
}
