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
        myButton.onClick.AddListener(() => { Debug.Log("LOG"); });
    }

    public void Log()
    {

    }
}