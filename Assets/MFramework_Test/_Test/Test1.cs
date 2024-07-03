using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    private void Start()
    {
        var res = LocalizationTable.LoadBytes();
        Debug.Log(res);
    }
}