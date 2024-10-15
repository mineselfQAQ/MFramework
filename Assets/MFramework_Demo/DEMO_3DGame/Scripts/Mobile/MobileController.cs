using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileController : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        CheckEnable();
    }

#if UNITY_EDITOR
    protected virtual void Update()
    {
        if (!Application.isPlaying)
        {
            CheckEnable();
        }
    }
#endif

    protected virtual void CheckEnable()
    {
#if UNITY_IOS || UNITY_ANDROID
        EnableRig(true);
#else
			EnableRig(false);
#endif
    }

    public virtual void EnableRig(bool value)
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(value);
        }
    }
}