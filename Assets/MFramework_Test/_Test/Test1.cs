using MFramework;
using MFramework.UI;
using System.Globalization;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    private void Start()
    {
        Debug.Log(CultureInfo.CurrentCulture.DisplayName);
    }
}