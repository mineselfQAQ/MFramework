using MFramework;
using MFramework.UI;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class Test1 : MonoBehaviour
{
    public MText text;
    public Button btn;

    private void Start()
    {
        btn.onClick.AddListener(() =>
        {
            text.FinishTextImmediately();
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            text.PlayText();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            text.PlayTextFastly();
        }
    }
}