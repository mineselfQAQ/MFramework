using MFramework;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    public GameObject root;

    private void Start()
    {
        root.DeleteAllChild(true);
    }
}
