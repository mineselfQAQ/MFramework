using MFramework;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    public Transform a;
    public Transform b;
    public Transform c;

    private void Start()
    {
        b.SetAsFirstSibling();
    }
}

namespace AAA
{
    public class A
    {

    }
    public class B
    {

    }

    
}