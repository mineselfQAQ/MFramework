using MFramework;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    private void Start()
    {
        string path = Path.GetFullPath("A/B/C");
        Debug.Log(path);
    }
}
