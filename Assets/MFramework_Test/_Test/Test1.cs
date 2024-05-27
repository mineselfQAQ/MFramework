using MFramework;
using MFramework.UI;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    public MButton myButton;  // 引用到Button组件

    void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var go = GameObject.Find("StartBtn");
            Debug.Log(go.GetComponent<MText>().mLocal);
        }
    }
}