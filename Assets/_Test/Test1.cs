using MFramework;
using MFramework.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    public Bouncer bouncer;

    private void Start()
    {
        Debug.Log(MPathUtility.IsFile("F:/MineselfDemo/MFramework/CORE/AB"));
        Debug.Log(MPathUtility.IsFile("F:/MineselfDemo/MFramework/CORE/AB/ABBuildSetting.xml"));
        Debug.Log(MPathUtility.IsFolder("F:/MineselfDemo/MFramework/CORE/AB"));
        Debug.Log(MPathUtility.IsFolder("F:/MineselfDemo/MFramework/CORE/AB/ABBuildSetting.xml"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            bouncer.StopBounce();
        }
    }
}