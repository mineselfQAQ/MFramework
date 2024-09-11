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

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            bouncer.StopBounce();
        }
    }
}