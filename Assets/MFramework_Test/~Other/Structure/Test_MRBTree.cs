using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_MRBTree : MonoBehaviour
{
    private void Start()
    {
        MRBTree tree = new MRBTree();
        tree.Add(17);
        tree.Add(25);
        tree.Add(33);
        tree.Add(38);
        tree.Add(46);
        tree.Add(50);
        tree.Add(55);
        tree.Add(72);
        tree.Add(76);
        tree.Add(80);
        tree.Add(88);

        tree.Remove(17);
        tree.Remove(80);
        tree.Remove(50);
        tree.Remove(25);

        tree.Print();
    }
}
