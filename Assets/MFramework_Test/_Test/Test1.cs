using MFramework;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    private void Start()
    {
        TMP_Text tmp = GameObject.Find("Text (TMP)").GetComponent<TMP_Text>();
        tmp.fontWeight = 0;
        tmp.CompareTag("1");
        Debug.Log(1);
    }
}
